namespace Kida.Models;

public sealed record EntitlementFeatureDeclaration(
    string Code,
    string DisplayName,
    string? Description,
    string ValueType = "boolean",
    int Flags = 0);
