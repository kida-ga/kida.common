namespace Kida.Models;
/// <summary>
/// Maps a coarse client scope to a fine-grained module action. The mapping sets
/// the maximum action ceiling for requests made through that client scope.
/// </summary>
public sealed record KidaScopeActionDefinition(string Scope, string Action);
