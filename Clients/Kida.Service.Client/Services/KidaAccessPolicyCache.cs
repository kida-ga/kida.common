using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Haley.Utils;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaAccessPolicyCache(
    IServiceScopeFactory scopeFactory,
    IOptions<KidaClientOptions> options,
    TimeProvider timeProvider) : IKidaAccessPolicyCache, IDisposable
{
    private readonly ConcurrentDictionary<PolicyKey, PolicyDefinitionCacheEntry> _definitions = new();
    private readonly ConcurrentDictionary<SubjectCacheKey, SubjectEntitlementCacheEntry> _subjects = new();
    private readonly ConcurrentDictionary<ClientGrantCacheKey, ClientGrantCacheEntry> _clientGrants = new();
    private readonly KidaClientOptions _options = options.Value;

    public async ValueTask<AccessPolicySnapshot> GetSnapshotAsync(Guid tenantId, string module, CancellationToken cancellationToken = default)
    {
        var definition = await GetDefinitionAsync(tenantId, module, cancellationToken).ConfigureAwait(false);
        return new(definition.TenantId, definition.Module, definition.Version, definition.GeneratedAt,
            definition.Roles, definition.RoleActions, [], _options.UserAudience);
    }

    public async ValueTask<AccessPolicyDefinitionSnapshot> GetDefinitionAsync(Guid tenantId, string module, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("A tenant identifier is required.", nameof(tenantId));
        var normalizedModule = NormalizeModule(module);
        var key = new PolicyKey(tenantId, _options.UserAudience, normalizedModule);
        var entry = _definitions.GetOrAdd(key, static _ => new PolicyDefinitionCacheEntry());
        var now = timeProvider.GetUtcNow();
        entry.LastAccessedAt = now;
        var snapshot = Volatile.Read(ref entry.Snapshot);
        if (snapshot is not null && IsFresh(entry, now)) return snapshot;

        await entry.RefreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            now = timeProvider.GetUtcNow();
            entry.LastAccessedAt = now;
            snapshot = Volatile.Read(ref entry.Snapshot);
            if (snapshot is not null && IsFresh(entry, now)) return snapshot;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IKidaClient>();
                var revision = await client.GetAccessPolicyVersionAsync(tenantId, normalizedModule, cancellationToken).ConfigureAwait(false);
                if (snapshot is null || snapshot.Version != revision.Version ||
                    !string.Equals(entry.RevisionHash, revision.RevisionHash, StringComparison.Ordinal))
                {
                    snapshot = await client.GetAccessPolicyDefinitionAsync(tenantId, normalizedModule, cancellationToken).ConfigureAwait(false);
                    Volatile.Write(ref entry.Snapshot, snapshot);
                }
                entry.RevisionHash = revision.RevisionHash;
                MarkValidated(entry, now);
                Trim(_definitions, _options.AccessPolicyCacheMaxEntries, now);
                return snapshot;
            }
            catch (Exception exception) when (CanServeStale(exception, snapshot, entry, now))
            {
                RetrySoon(entry, now);
                return snapshot!;
            }
        }
        finally
        {
            entry.RefreshLock.Release();
        }
    }

    public async ValueTask<SubjectEntitlementSnapshot> GetSubjectSnapshotAsync(
        Guid tenantId,
        string module,
        IReadOnlyCollection<AccessSubjectRef> subjects,
        IReadOnlyCollection<AccessScopeRef> scopePath,
        CancellationToken cancellationToken = default)
    {
        var normalizedModule = NormalizeModule(module);
        var normalizedSubjects = NormalizeSubjects(subjects);
        var normalizedPath = NormalizePath(scopePath);
        var key = new SubjectCacheKey(tenantId, _options.UserAudience, normalizedModule,
            HashKey(normalizedSubjects.Select(value => $"{value.Type}:{value.Id:N}")),
            HashKey(normalizedPath.Select(value => $"{value.Type}:{value.Id?.ToString("N") ?? "tenant"}")));
        var entry = _subjects.GetOrAdd(key, static _ => new SubjectEntitlementCacheEntry());
        var now = timeProvider.GetUtcNow();
        entry.LastAccessedAt = now;
        var snapshot = Volatile.Read(ref entry.Snapshot);
        if (snapshot is not null && IsFresh(entry, now)) return snapshot;

        await entry.RefreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            now = timeProvider.GetUtcNow();
            entry.LastAccessedAt = now;
            snapshot = Volatile.Read(ref entry.Snapshot);
            if (snapshot is not null && IsFresh(entry, now)) return snapshot;
            var query = new SubjectEntitlementQuery(_options.UserAudience, normalizedModule, normalizedSubjects, normalizedPath);
            try
            {
                using var scope = scopeFactory.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IKidaClient>();
                var revision = await client.GetSubjectEntitlementRevisionAsync(tenantId, query, cancellationToken).ConfigureAwait(false);
                if (snapshot is null || !string.Equals(entry.RevisionHash, revision.RevisionHash, StringComparison.Ordinal))
                {
                    snapshot = await client.GetSubjectEntitlementSnapshotAsync(tenantId, query, cancellationToken).ConfigureAwait(false);
                    Volatile.Write(ref entry.Snapshot, snapshot);
                }
                entry.RevisionHash = revision.RevisionHash;
                MarkValidated(entry, now);
                Trim(_subjects, _options.SubjectEntitlementCacheMaxEntries, now);
                return snapshot;
            }
            catch (Exception exception) when (CanServeStale(exception, snapshot, entry, now))
            {
                RetrySoon(entry, now);
                return snapshot!;
            }
        }
        finally
        {
            entry.RefreshLock.Release();
        }
    }

    public async ValueTask<AccessDecision> EvaluateAsync(AccessDecisionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!string.Equals(request.Resource?.Trim(), _options.UserAudience, StringComparison.Ordinal))
            return new(false, "resource_audience_mismatch", []);
        var authorizedClientId = request.AuthorizedClientId ?? _options.ClientId;
        if (authorizedClientId == Guid.Empty)
            return new(false, "client_grant_missing", []);
        var clientGrant = await GetClientGrantAsync(authorizedClientId, _options.UserAudience, cancellationToken).ConfigureAwait(false);
        if (!string.Equals(clientGrant.Status, "active", StringComparison.Ordinal))
            return new(false, "client_grant_inactive", []);
        var subjects = NormalizeSubjects([new(request.SubjectType, request.SubjectId), .. request.Subjects ?? []]);
        var path = request.ScopePath is { Count: > 0 }
            ? NormalizePath(request.ScopePath)
            : NormalizePath(string.Equals(request.ScopeType, "tenant", StringComparison.OrdinalIgnoreCase)
                ? [new AccessScopeRef("tenant", null)]
                : [new AccessScopeRef("tenant", null), new AccessScopeRef(request.ScopeType, request.ScopeId)]);
        var definition = await GetDefinitionAsync(request.TenantId, request.Module, cancellationToken).ConfigureAwait(false);
        var clientScopes = clientGrant.AllowedScopes.ToHashSet(StringComparer.Ordinal);
        if (definition.ScopeActions is null || !definition.ScopeActions.Any(mapping =>
                string.Equals(mapping.Action, request.Action, StringComparison.Ordinal) &&
                clientScopes.Contains(mapping.Scope)))
            return new(false, "client_scope_denied", []);
        var entitlements = await GetSubjectSnapshotAsync(request.TenantId, request.Module, subjects, path, cancellationToken).ConfigureAwait(false);
        var evaluatedAt = timeProvider.GetUtcNow();
        var nearestOrdinaryDepth = entitlements.Assignments
            .Where(assignment => !AccessAssignmentFlags.Has(assignment.Flags, AccessAssignmentFlags.Protected))
            .Where(assignment => AssignmentIsActive(assignment, evaluatedAt))
            .Select(assignment => AssignmentScopeDepth(assignment, path))
            .Where(depth => depth >= 0)
            .DefaultIfEmpty(-1)
            .Max();
        var grantsByRole = definition.RoleActions
            .Where(grant => string.Equals(grant.Action, request.Action, StringComparison.OrdinalIgnoreCase))
            .ToLookup(grant => grant.RoleId);
        var candidates = entitlements.Assignments
            .Where(assignment => AccessAssignmentFlags.Has(assignment.Flags, AccessAssignmentFlags.Protected) ||
                                 AssignmentScopeDepth(assignment, path) == nearestOrdinaryDepth)
            .SelectMany(assignment => grantsByRole[assignment.RoleId].Select(grant =>
                new AccessGrant(assignment.AssignmentId, grant.Effect, assignment.ScopeType, assignment.ScopeId,
                    assignment.ValidFrom, assignment.ValidUntil, grant.ConditionPayload, assignment.Flags)))
            .ToArray();
        if (candidates.Length == 0 && nearestOrdinaryDepth >= 0)
            return new(false, "nearest_role_action_missing", []);
        return AccessPolicy.Evaluate(request with { Subjects = subjects, ScopePath = path },
            candidates, evaluatedAt, ExactConditionMatches);
    }

    private async ValueTask<ClientResourceGrantSnapshot> GetClientGrantAsync(
        Guid clientId,
        string audience,
        CancellationToken cancellationToken)
    {
        var key = new ClientGrantCacheKey(clientId, audience);
        var entry = _clientGrants.GetOrAdd(key, static _ => new ClientGrantCacheEntry());
        var now = timeProvider.GetUtcNow();
        entry.LastAccessedAt = now;
        var snapshot = Volatile.Read(ref entry.Snapshot);
        if (snapshot is not null && IsFresh(entry, now)) return snapshot;

        await entry.RefreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            now = timeProvider.GetUtcNow();
            entry.LastAccessedAt = now;
            snapshot = Volatile.Read(ref entry.Snapshot);
            if (snapshot is not null && IsFresh(entry, now)) return snapshot;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IKidaClient>();
                var revision = await client.GetClientResourceGrantRevisionAsync(clientId, audience, cancellationToken).ConfigureAwait(false);
                if (revision.ClientId != clientId || !string.Equals(revision.Audience, audience, StringComparison.Ordinal))
                    throw new InvalidOperationException("Kida returned a client grant for a different security boundary.");
                if (snapshot is null || snapshot.Version != revision.Version ||
                    !string.Equals(snapshot.Status, revision.Status, StringComparison.Ordinal) ||
                    !string.Equals(entry.RevisionHash, revision.RevisionHash, StringComparison.Ordinal))
                {
                    snapshot = await client.GetClientResourceGrantSnapshotAsync(clientId, audience, cancellationToken).ConfigureAwait(false);
                    if (snapshot.ClientId != clientId || !string.Equals(snapshot.Audience, audience, StringComparison.Ordinal))
                        throw new InvalidOperationException("Kida returned a client grant for a different security boundary.");
                    Volatile.Write(ref entry.Snapshot, snapshot);
                }
                entry.RevisionHash = revision.RevisionHash;
                MarkValidated(entry, now);
                Trim(_clientGrants, _options.ClientGrantCacheMaxEntries, now);
                return snapshot;
            }
            catch (Exception exception) when (CanServeStale(exception, snapshot, entry, now))
            {
                RetrySoon(entry, now);
                return snapshot!;
            }
        }
        finally
        {
            entry.RefreshLock.Release();
        }
    }

    public void Invalidate(Guid tenantId, string module)
    {
        if (tenantId == Guid.Empty || string.IsNullOrWhiteSpace(module)) return;
        var normalized = NormalizeModule(module);
        if (_definitions.TryGetValue(new(tenantId, _options.UserAudience, normalized), out var definition))
            WriteNextVersionCheck(definition, DateTimeOffset.MinValue);
        foreach (var pair in _subjects.Where(pair => pair.Key.TenantId == tenantId && pair.Key.Resource == _options.UserAudience && pair.Key.Module == normalized))
            WriteNextVersionCheck(pair.Value, DateTimeOffset.MinValue);
    }

    private IReadOnlyCollection<AccessSubjectRef> NormalizeSubjects(IEnumerable<AccessSubjectRef> values)
    {
        var subjects = values.Select(value => new AccessSubjectRef(
                NormalizeCode(value.Type, 24, "subject type"),
                value.Id == Guid.Empty ? throw new ArgumentException("A subject identifier is required.") : value.Id))
            .Distinct().OrderBy(value => value.Type, StringComparer.Ordinal).ThenBy(value => value.Id).ToArray();
        if (subjects.Length is < 1 || subjects.Length > _options.SubjectSetMaxCount)
            throw new ArgumentException($"Between 1 and {_options.SubjectSetMaxCount} subjects are required.");
        return subjects;
    }

    private static IReadOnlyCollection<AccessScopeRef> NormalizePath(IEnumerable<AccessScopeRef> values)
    {
        var path = values.Select(value => new AccessScopeRef(NormalizeCode(value.Type, 40, "scope type"), value.Id)).ToArray();
        if (path.Length is < 1 or > 32 || !string.Equals(path[0].Type, "tenant", StringComparison.Ordinal) ||
            path[0].Id is not null || path.Skip(1).Any(value => value.Id is null || value.Type == "tenant"))
            throw new ArgumentException("The scope path must start with tenant and contain at most 32 identified child levels.");
        return path;
    }

    private static string NormalizeCode(string value, int maximumLength, string label)
    {
        var normalized = value?.Trim().Normalize().ToLowerInvariant();
        return normalized is { Length: >= 1 } && normalized.Length <= maximumLength &&
               normalized.All(character => character is >= 'a' and <= 'z' or >= '0' and <= '9' or '.' or '_' or '-')
            ? normalized
            : throw new ArgumentException($"A valid {label} is required.");
    }

    private static string HashKey(IEnumerable<string> values) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('|', values)))).ToLowerInvariant();

    private static bool ExactConditionMatches(string conditionPayload, IReadOnlyDictionary<string, string> attributes)
    {
        try
        {
            if (!conditionPayload.IsValidJson(tryParse: true)) return false;
            var required = conditionPayload.FromJson<Dictionary<string, string>>();
            return required is not null && required.All(pair => attributes.TryGetValue(pair.Key, out var actual) &&
                string.Equals(pair.Value, actual, StringComparison.Ordinal));
        }
        catch { return false; }
    }

    private static bool AssignmentIsActive(RoleAssignmentInfo assignment, DateTimeOffset evaluatedAt) =>
        string.Equals(assignment.Status, "active", StringComparison.Ordinal) &&
        assignment.ValidFrom <= evaluatedAt &&
        (assignment.ValidUntil is null || assignment.ValidUntil > evaluatedAt);

    private static int AssignmentScopeDepth(RoleAssignmentInfo assignment, IReadOnlyCollection<AccessScopeRef> scopePath)
    {
        var path = scopePath as IReadOnlyList<AccessScopeRef> ?? scopePath.ToArray();
        for (var index = 0; index < path.Count; index++)
        {
            if (!string.Equals(assignment.ScopeType, path[index].Type, StringComparison.OrdinalIgnoreCase) ||
                assignment.ScopeId != path[index].Id) continue;
            return index == path.Count - 1 ||
                   AccessAssignmentFlags.Has(assignment.Flags, AccessAssignmentFlags.AppliesToDescendants)
                ? index
                : -1;
        }
        return -1;
    }

    private static string NormalizeModule(string module) => NormalizeCode(module, 150, "Kida Access module code");

    private bool IsFresh(CacheEntry entry, DateTimeOffset now) =>
        entry.LastAccessedAt.AddSeconds(_options.AccessPolicyCacheSlidingSeconds) > now && ReadNextVersionCheck(entry) > now;

    private void MarkValidated(CacheEntry entry, DateTimeOffset now)
    {
        entry.LoadedAt = now;
        entry.LastAccessedAt = now;
        WriteNextVersionCheck(entry, now.AddSeconds(_options.AccessPolicyVersionCheckSeconds));
    }

    private bool CanServeStale<T>(Exception exception, T? snapshot, CacheEntry entry, DateTimeOffset now) where T : class
    {
        if (snapshot is null || entry.LoadedAt.AddSeconds(_options.AccessPolicyMaxStalenessSeconds) <= now)
            return false;
        if (exception is OperationCanceledException)
            return false;
        if (exception is not HttpRequestException httpException)
            return false;
        return httpException.StatusCode is null or HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests ||
               (int)httpException.StatusCode.Value >= 500;
    }

    private void RetrySoon(CacheEntry entry, DateTimeOffset now) =>
        WriteNextVersionCheck(entry, now.AddSeconds(Math.Min(10, _options.AccessPolicyVersionCheckSeconds)));

    private void Trim<TKey, TEntry>(ConcurrentDictionary<TKey, TEntry> entries, int maximum, DateTimeOffset now)
        where TKey : notnull where TEntry : CacheEntry
    {
        if (entries.Count <= maximum) return;
        var expiredBefore = now.AddSeconds(-_options.AccessPolicyCacheSlidingSeconds);
        foreach (var pair in entries.Where(pair => pair.Value.LastAccessedAt <= expiredBefore).Take(entries.Count - maximum))
            if (entries.TryRemove(pair.Key, out var removed)) removed.RefreshLock.Dispose();
        if (entries.Count <= maximum) return;
        foreach (var pair in entries.OrderBy(pair => pair.Value.LastAccessedAt).Take(Math.Max(1, entries.Count - maximum)))
            if (entries.TryRemove(pair.Key, out var removed)) removed.RefreshLock.Dispose();
    }

    public void Dispose()
    {
        foreach (var entry in _definitions.Values) entry.RefreshLock.Dispose();
        foreach (var entry in _subjects.Values) entry.RefreshLock.Dispose();
        foreach (var entry in _clientGrants.Values) entry.RefreshLock.Dispose();
    }

    private static DateTimeOffset ReadNextVersionCheck(CacheEntry entry) =>
        new(Interlocked.Read(ref entry.NextVersionCheckUtcTicks), TimeSpan.Zero);

    private static void WriteNextVersionCheck(CacheEntry entry, DateTimeOffset value) =>
        Interlocked.Exchange(ref entry.NextVersionCheckUtcTicks, value.UtcTicks);
}
