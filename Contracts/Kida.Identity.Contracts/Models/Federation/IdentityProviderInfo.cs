using Haley.Abstractions;

namespace Kida.Models;
public sealed record IdentityProviderInfo(
    Guid ProviderId,
    string Code,
    FederationProtocol Protocol,
    string Issuer,
    string DisplayName,
    string Status,
    Guid? TenantId,
    string Configuration,
    IReadOnlyCollection<string> AuthoritativeDomains,
    DateTimeOffset ModifiedAt,
    IReadOnlyCollection<string>? SigningCertificates = null);
