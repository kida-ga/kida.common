namespace Kida.Models;

public sealed record EntitlementProductReleaseInfo(
    Guid ReleaseId,
    Guid ProductId,
    string Version,
    string ContentHash,
    string Status,
    DateTimeOffset FirstRegisteredAt,
    DateTimeOffset LastRegisteredAt,
    int FeatureCount,
    int MeterCount);
