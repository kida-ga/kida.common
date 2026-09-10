using Haley.Utils;

namespace Kida.Models;
public sealed record KidaModuleCompositionResult(RegisterOAuthScopesRequest ScopeRequest, KidaModuleCatalogDefinition Module, IReadOnlyCollection<string> Warnings);
