using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
namespace Kida.Utils;
public static class AccessPolicy
{
    public static AccessDecision Evaluate(AccessDecisionRequest request, IEnumerable<AccessGrant> candidates, DateTimeOffset evaluatedAt, Func<string, IReadOnlyDictionary<string, string>, bool> evaluateCondition)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(candidates);
        ArgumentNullException.ThrowIfNull(evaluateCondition);
        var path = BuildScopePath(request);
        var matched = candidates
            .Where(candidate => IsActive(candidate, evaluatedAt))
            .Select(candidate => (Grant: candidate, Depth: ScopeDepth(candidate, path)))
            .Where(candidate => candidate.Depth >= 0)
            .Where(candidate => ConditionMatches(candidate.Grant, request.Attributes, evaluateCondition))
            .ToArray();

        var protectedDenied = matched.Where(candidate =>
            AccessAssignmentFlags.Has(candidate.Grant.Flags, AccessAssignmentFlags.Protected) &&
            candidate.Grant.Effect == AccessEffect.Deny).ToArray();
        if (protectedDenied.Length > 0)
        {
            return new(false, "protected_deny", protectedDenied.Select(candidate => candidate.Grant.AssignmentId).Distinct().ToArray());
        }

        var protectedAllowed = matched.Where(candidate =>
            AccessAssignmentFlags.Has(candidate.Grant.Flags, AccessAssignmentFlags.Protected) &&
            candidate.Grant.Effect == AccessEffect.Allow).ToArray();
        if (protectedAllowed.Length > 0)
        {
            return new(true, "protected_allow", protectedAllowed.Select(candidate => candidate.Grant.AssignmentId).Distinct().ToArray());
        }

        var ordinary = matched.Where(candidate =>
            !AccessAssignmentFlags.Has(candidate.Grant.Flags, AccessAssignmentFlags.Protected)).ToArray();
        if (ordinary.Length == 0)
        {
            return new(false, "no_matching_grant", Array.Empty<Guid>());
        }

        var nearestDepth = ordinary.Max(candidate => candidate.Depth);
        var nearest = ordinary.Where(candidate => candidate.Depth == nearestDepth).ToArray();
        var denied = nearest.Where(candidate => candidate.Grant.Effect == AccessEffect.Deny).ToArray();
        if (denied.Length > 0)
        {
            return new(false, "nearest_scope_deny", denied.Select(candidate => candidate.Grant.AssignmentId).Distinct().ToArray());
        }

        var allowed = nearest.Where(candidate => candidate.Grant.Effect == AccessEffect.Allow).ToArray();
        return allowed.Length > 0
            ? new(true, "nearest_scope_allow", allowed.Select(candidate => candidate.Grant.AssignmentId).Distinct().ToArray())
            : new(false, "no_role_action", Array.Empty<Guid>());
    }

    private static bool IsActive(AccessGrant candidate, DateTimeOffset evaluatedAt) => candidate.ValidFrom <= evaluatedAt && (candidate.ValidUntil is null || candidate.ValidUntil > evaluatedAt);
    private static IReadOnlyList<AccessScopeRef> BuildScopePath(AccessDecisionRequest request)
    {
        var path = new List<AccessScopeRef> { new("tenant", null) };
        if (request.ScopePath is { Count: > 0 })
        {
            foreach (var level in request.ScopePath)
            {
                if (string.Equals(level.Type, "tenant", StringComparison.OrdinalIgnoreCase)) continue;
                path.Add(level);
            }
        }
        else if (!string.Equals(request.ScopeType, "tenant", StringComparison.OrdinalIgnoreCase))
        {
            path.Add(new(request.ScopeType, request.ScopeId));
        }

        return path;
    }

    private static int ScopeDepth(AccessGrant candidate, IReadOnlyList<AccessScopeRef> path)
    {
        var depth = -1;
        for (var index = 0; index < path.Count; index++)
        {
            if (string.Equals(candidate.ScopeType, path[index].Type, StringComparison.OrdinalIgnoreCase) &&
                candidate.ScopeId == path[index].Id)
            {
                depth = index;
                break;
            }
        }

        if (depth < 0) return -1;
        return depth == path.Count - 1 ||
               AccessAssignmentFlags.Has(candidate.Flags, AccessAssignmentFlags.AppliesToDescendants)
            ? depth
            : -1;
    }

    private static bool ConditionMatches(AccessGrant candidate, IReadOnlyDictionary<string, string>? attributes, Func<string, IReadOnlyDictionary<string, string>, bool> evaluateCondition)
    {
        if (string.IsNullOrWhiteSpace(candidate.ConditionPayload))
        {
            return true;
        }

        return evaluateCondition(candidate.ConditionPayload, attributes ?? EmptyAttributes.Instance);
    }

}
