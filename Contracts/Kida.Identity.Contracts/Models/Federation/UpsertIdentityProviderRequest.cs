using Haley.Abstractions;

namespace Kida.Models;
public sealed record UpsertIdentityProviderRequest(
    string Code,
    FederationProtocol Protocol,
    string Issuer,
    string DisplayName,
    string Configuration,
    IReadOnlyCollection<string>? AuthoritativeDomains = null,
    Guid? TenantId = null,
    string Status = "active",
    IReadOnlyCollection<string>? SigningCertificates = null);
