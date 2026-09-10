namespace Kida.Models;

public sealed record KidaAuthEdgeClientTokenRequest(
    string ClientIdentifier,
    string ClientSecret);
