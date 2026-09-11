namespace Kida.Models;

public sealed record EntitlementMeterDeclaration(
    string Code,
    string FeatureCode,
    string Unit,
    string Aggregation = "sum",
    string Period = "month");
