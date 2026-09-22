using Haley.Models;
using System.Text.Json.Serialization;
using Haley.Abstractions;

namespace Kida.Models;
public sealed record IdentityProviderInfo(
    Guid ProviderId,
    string Code,
    FederationProtocol Protocol,
    string Issuer,
    string DisplayName,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status,
    Guid? TenantId,
    string Configuration,
    IReadOnlyCollection<string> AuthoritativeDomains,
    DateTimeOffset ModifiedAt,
    IReadOnlyCollection<string>? SigningCertificates = null);
