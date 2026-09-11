using System.Text.Json;

namespace Kida.Models;

public sealed record RegisterAppCatalogRequest(
    string Code,
    string DisplayName,
    string? Description,
    string Type,
    string Version,
    string Channel,
    Guid? PublisherTenantId,
    string PublisherAudience,
    JsonElement Manifest,
    string KeyId,
    string Signature,
    IReadOnlyCollection<AppEndpointDeclaration> Endpoints,
    IReadOnlyCollection<AppContributionDeclaration> Contributions,
    string? RequiredProductCode,
    string? RequiredFeatureCode);
