namespace Kida.Models;
/// <summary>
/// A code-owned security catalog for one stable functional module. Module
/// identity survives service/capability regrouping; a separate version identity
/// records each observed implementation version.
/// </summary>
public sealed record KidaModuleCatalogDefinition(string OwnerFamily, string ModuleCode, string ModuleVersion, string MetadataFile, IReadOnlyCollection<KidaScopeDefinition> Scopes, IReadOnlyCollection<KidaActionDefinition> Actions, IReadOnlyCollection<KidaScopeActionDefinition> ScopeActions);
