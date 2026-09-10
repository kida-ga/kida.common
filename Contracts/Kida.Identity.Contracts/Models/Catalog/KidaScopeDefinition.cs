namespace Kida.Models;
/// <summary>A coarse OAuth permission exposed by one module to service clients.</summary>
public sealed record KidaScopeDefinition(string Code, KidaScopeKind Kind = KidaScopeKind.Exact, string? Parameter = null, string? ParameterSource = null);
