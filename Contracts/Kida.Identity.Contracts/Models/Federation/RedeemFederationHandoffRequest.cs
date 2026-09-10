using Haley.Abstractions;

namespace Kida.Models;
public sealed record RedeemFederationHandoffRequest(Guid ClientId, string Code, string CodeVerifier = "");
