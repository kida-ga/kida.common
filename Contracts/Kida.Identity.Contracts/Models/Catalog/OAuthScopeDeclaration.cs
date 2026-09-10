namespace Kida.Models;
public sealed record OAuthScopeDeclaration(string Code, string DisplayName, string? Description = null, KidaScopeKind Kind = KidaScopeKind.Exact, string? Parameter = null, string? ParameterSource = null);
