using Haley.Models;
using System.Text.Json.Serialization;
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
    [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status = IdentityRecordStatus.Active,
    IReadOnlyCollection<string>? SigningCertificates = null);
