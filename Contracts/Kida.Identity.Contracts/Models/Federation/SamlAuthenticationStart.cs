using Haley.Abstractions;

namespace Kida.Models;
public sealed record SamlAuthenticationStart(Guid RequestId, string IdentityProviderUrl, string SamlRequest, string RelayState, DateTimeOffset ExpiresAt, string AuthorizationUrl = "");
