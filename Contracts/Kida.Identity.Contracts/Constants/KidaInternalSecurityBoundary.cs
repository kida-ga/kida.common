using Haley.Abstractions;
using Kida.Models;
using System.Text.Json.Serialization;

namespace Kida.Constants;
/// <summary>
/// Defines the Kida-owned audience and catalog namespaces. These identifiers are
/// stable security boundaries, not deployment labels. Remote module publishers
/// cannot claim them; Kida's in-process providers publish them during startup.
/// </summary>
public static class KidaInternalSecurityBoundary
{
    public const string ServiceAudience = "kida-service";
    public const string OwnerFamily = "kida";
    public const string AdminClientIdentifier = "kida-admin-ui";
    public const string ManagementScopePrefix = "management.";
    public const string TenancyScopePrefix = "tenancy.";
    public static bool IsInternalAudience(string? value) => EqualsNormalized(value, ServiceAudience);
    public static bool IsReservedOwner(string? value) => EqualsNormalized(value, OwnerFamily);
    public static bool IsAdminClientIdentifier(string? value) => EqualsNormalized(value, AdminClientIdentifier);
    public static bool IsReservedModule(string? value)
    {
        var normalized = Normalize(value);
        return StartsWith(normalized, "kida.");
    }

    public static bool IsReservedScope(string? value)
    {
        var normalized = Normalize(value);
        return StartsWith(normalized, "identity.") || StartsWith(normalized, "access.") || IsTenancyScope(normalized) || IsManagementScope(normalized);
    }

    public static bool IsTenancyScope(string? value) =>
        StartsWith(Normalize(value), TenancyScopePrefix);

    public static bool ContainsTenancyScope(IEnumerable<string>? scopes) =>
        scopes?.Any(IsTenancyScope) == true;

    public static bool ContainsTenancyScope(IEnumerable<OAuthClientResourceGrant>? grants) =>
        grants?.Any(grant => ContainsTenancyScope(grant.AllowedScopes)) == true;

    public static bool IsManagementScope(string? value) =>
        StartsWith(Normalize(value), ManagementScopePrefix);

    public static bool ContainsManagementScope(IEnumerable<string>? scopes) =>
        scopes?.Any(IsManagementScope) == true;

    public static bool ContainsManagementScope(IEnumerable<OAuthClientResourceGrant>? grants) =>
        grants?.Any(grant => ContainsManagementScope(grant.AllowedScopes)) == true;

    /// <summary>
    /// Scopes that are never implied by a kida-service wildcard. They require a
    /// separately and deliberately configured credential or explicit grant.
    /// </summary>
    public static bool IsProtectedWildcardScope(string? value)
    {
        var normalized = Normalize(value);
        return IsManagementScope(normalized) ||
               string.Equals(normalized, KidaIdentityScopes.ClientsManage, StringComparison.Ordinal) ||
               string.Equals(normalized, "access.protected.manage", StringComparison.Ordinal);
    }

    public static bool IsManagementClient(OAuthClientInfo? client) =>
        client is not null &&
        (IsAdminClientIdentifier(client.ClientIdentifier) ||
         ContainsManagementScope(client.AllowedScopes) ||
         ContainsManagementScope(client.ResourceGrants));

    public static bool IsManagementClientRegistration(RegisterOAuthClientRequest request) =>
        IsAdminClientIdentifier(request.ClientIdentifier) ||
        ContainsManagementScope(request.AllowedScopes) ||
        ContainsManagementScope(request.ResourceGrants);

    public static bool MayPublishExternalScopeCatalog(RegisterOAuthScopesRequest request) => !IsInternalAudience(request.Audience) && !IsReservedOwner(request.OwnerFamily) && !IsReservedModule(request.ModuleCode) && request.Scopes is not null && request.Scopes.All(scope => !IsReservedScope(scope.Code));
    private static bool EqualsNormalized(string? value, string expected) => string.Equals(Normalize(value), expected, StringComparison.Ordinal);
    private static bool StartsWith(string value, string prefix) => value.StartsWith(prefix, StringComparison.Ordinal);
    private static string Normalize(string? value) => value?.Trim().Normalize().ToLowerInvariant() ?? string.Empty;
}
