namespace Kida.Models;

public sealed record AcquireLicenseRequest(
    Guid TenantId,
    string Audience,
    Guid PoolId,
    string SubjectType,
    Guid SubjectId,
    Guid RequestId,
    int LeaseSeconds = 300);
