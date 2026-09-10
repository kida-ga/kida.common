using Haley.Abstractions;

namespace Kida.Models;
public sealed record CompleteSamlAuthenticationRequest(
    string SamlResponse,
    string RelayState,
    Guid? ClientId = null);
