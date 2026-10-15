namespace Kida.Models;

/// <summary>
/// Removes a role that no longer has an active assignment. The role is retired rather than
/// physically deleted so assignment and grant history keeps its reference.
/// </summary>
public sealed record DeleteRoleRequest(
    Guid TenantId,
    Guid RoleId,
    Guid? ActorId = null,
    string Resource = "");
