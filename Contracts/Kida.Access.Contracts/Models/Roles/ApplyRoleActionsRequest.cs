namespace Kida.Models;

public sealed record ApplyRoleActionsRequest(
    Guid TenantId,
    Guid RoleId,
    IReadOnlyCollection<RoleActionChange> Changes,
    Guid? ActorId = null,
    string Resource = "");
