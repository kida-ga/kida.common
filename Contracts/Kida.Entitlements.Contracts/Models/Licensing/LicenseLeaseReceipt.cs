namespace Kida.Models;

public sealed record LicenseLeaseReceipt(
    Guid LeaseId,
    Guid PoolId,
    Guid SubjectId,
    DateTimeOffset AcquiredAt,
    DateTimeOffset ExpiresAt,
    string Status);
