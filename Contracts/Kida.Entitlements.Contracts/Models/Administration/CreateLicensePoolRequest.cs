namespace Kida.Models;

public sealed record CreateLicensePoolRequest(
    Guid TenantId,
    Guid SubscriptionId,
    Guid? FeatureId,
    string Type,
    int Quantity,
    string? DisplayName);
