using Haley.Abstractions;

namespace Kida.Models;
public sealed record BeginSamlAuthenticationRequest(Guid ClientId, string Resource, string ProviderCode, string ReturnUri, string State = "", string CodeChallenge = "");
