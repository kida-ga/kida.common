namespace Kida.Models;

public sealed record RegisterEntitlementCatalogRequest(
    Guid DeploymentId,
    string ProductCode,
    string DisplayName,
    string? Description,
    string Audience,
    string Version,
    IReadOnlyCollection<EntitlementFeatureDeclaration> Features,
    IReadOnlyCollection<EntitlementMeterDeclaration> Meters);
