namespace Kida.Models;

public sealed record RevokeRoleAssignmentRequest(
    Guid TenantId,
    Guid AssignmentId,
    Guid? ActorId = null,
    string? Reason = null,
    string Resource = "");
