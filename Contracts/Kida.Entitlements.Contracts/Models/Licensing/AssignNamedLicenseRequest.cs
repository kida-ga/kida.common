namespace Kida.Models;

public sealed record AssignNamedLicenseRequest(
    Guid TenantId,
    Guid PoolId,
    string SubjectType,
    Guid SubjectId);
