namespace Kida.Models;

/// <summary>Tenant/resource/module role and action definitions; never contains subject assignments.</summary>
public sealed record AccessPolicyDefinitionSnapshot(
    Guid TenantId,
    string Module,
    ulong Version,
    string? RevisionHash,
    DateTimeOffset GeneratedAt,
    IReadOnlyCollection<AccessRoleInfo> Roles,
    IReadOnlyCollection<AccessRoleActionInfo> RoleActions,
    IReadOnlyCollection<AccessScopeActionGrantInfo>? ScopeActions = null,
    string Resource = "");
