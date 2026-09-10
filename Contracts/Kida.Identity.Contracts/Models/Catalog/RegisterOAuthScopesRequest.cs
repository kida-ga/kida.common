namespace Kida.Models;
public sealed record RegisterOAuthScopesRequest(string OwnerFamily, string ModuleCode, string ModuleVersion, string ModuleDisplayName, string? ModuleDescription, string Audience, IReadOnlyCollection<OAuthScopeDeclaration> Scopes, bool OverwriteDescriptions = true);
