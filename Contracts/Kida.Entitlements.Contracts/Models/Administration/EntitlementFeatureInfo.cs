namespace Kida.Models;

public sealed record EntitlementFeatureInfo(
    Guid FeatureId,
    Guid ProductId,
    string Code,
    string DisplayName,
    string? Description,
    string ValueType,
    string LicenseMode,
    string Status);
