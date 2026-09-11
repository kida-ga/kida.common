namespace Kida.Models;

public sealed record LicenseAssignmentInfo(
    Guid AssignmentId,
    Guid TenantId,
    Guid PoolId,
    string SubjectType,
    Guid SubjectId,
    DateTimeOffset AssignedAt,
    Guid? AssignedById,
    DateTimeOffset? ReleasedAt);
