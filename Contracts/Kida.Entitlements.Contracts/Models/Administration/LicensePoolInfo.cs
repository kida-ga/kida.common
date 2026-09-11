namespace Kida.Models;

public sealed record LicensePoolInfo(
    Guid PoolId,
    Guid TenantId,
    Guid SubscriptionId,
    Guid? FeatureId,
    string Type,
    int Quantity,
    string Status,
    int Assigned,
    int Leased);
