namespace Kida.Models;

public sealed record KidaAuthEdgeSamlStartRequest(
    string ProviderCode,
    string ReturnUri,
    string State,
    string CodeChallenge);
