namespace Kida.Models;

public sealed record EntitlementMeterInfo(
    Guid MeterId,
    Guid ProductId,
    Guid? FeatureId,
    string Code,
    string Unit,
    string Aggregation,
    string Period,
    string Status);
