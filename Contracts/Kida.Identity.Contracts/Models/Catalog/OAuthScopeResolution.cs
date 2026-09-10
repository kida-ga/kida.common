namespace Kida.Models;
public sealed record OAuthScopeResolution(IReadOnlyCollection<string> ValidScopes, IReadOnlyCollection<string> InvalidScopes);
