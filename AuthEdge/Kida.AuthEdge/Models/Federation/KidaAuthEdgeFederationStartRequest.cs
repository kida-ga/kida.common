namespace Kida.Models;

/// <summary>Browser login input. The product's configured client and audience supply the application binding.</summary>
public sealed record KidaAuthEdgeFederationStartRequest(
    string ReturnUri,
    string State,
    string CodeChallenge,
    string ProviderCode = "",
    string EmailOrDomain = "");
