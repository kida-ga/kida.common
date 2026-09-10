using Haley.Utils;

namespace Kida.Utils;
public static class KidaScopeCatalogComposer
{
    public static KidaModuleCompositionResult Compose(IKidaModuleCatalogProvider provider, string audience, KidaScopeCatalogOptions options, string? baseDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(options);
        var module = provider.GetModuleCatalog();
        if (!KidaResourceAudience.TryNormalize(audience, out var normalizedAudience))
        {
            throw new InvalidOperationException($"Module provider '{provider.GetType().FullName}' received an invalid service audience.");
        }

        if (string.IsNullOrWhiteSpace(module.MetadataFile) || !string.Equals(module.MetadataFile, Path.GetFileName(module.MetadataFile), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Module provider '{provider.GetType().FullName}' must declare a safe metadata filename.");
        }

        ValidateCompiledCatalog(provider, module);
        var root = string.IsNullOrWhiteSpace(baseDirectory) ? AppContext.BaseDirectory : baseDirectory;
        var warnings = new List<string>();
        var scopeMetadata = new Dictionary<string, KidaScopeMetadata>(StringComparer.Ordinal);
        var actionMetadata = new Dictionary<string, KidaActionMetadata>(StringComparer.Ordinal);
        var metadataFile = new KidaScopeMetadataFile();
        LoadMetadata(Path.Combine(root, "ScopeDefaults", module.MetadataFile), required: true, scopeMetadata, actionMetadata, metadataFile, warnings);
        if (!string.IsNullOrWhiteSpace(options.MetadataRoot))
        {
            var overrideRoot = Path.IsPathRooted(options.MetadataRoot) ? options.MetadataRoot : Path.Combine(root, options.MetadataRoot);
            LoadMetadata(Path.Combine(overrideRoot, module.MetadataFile), required: false, scopeMetadata, actionMetadata, metadataFile, warnings);
        }

        var knownScopes = module.Scopes.Select(scope => NormalizeCode(scope.Code)).ToHashSet(StringComparer.Ordinal);
        var knownActions = module.Actions.Select(action => NormalizeCode(action.Code)).ToHashSet(StringComparer.Ordinal);
        foreach (var unknown in scopeMetadata.Keys.Where(code => !knownScopes.Contains(code)).Order(StringComparer.Ordinal))
        {
            warnings.Add($"Module metadata '{module.MetadataFile}' contains unknown scope '{unknown}' and was ignored.");
        }

        foreach (var unknown in actionMetadata.Keys.Where(code => !knownActions.Contains(code)).Order(StringComparer.Ordinal))
        {
            warnings.Add($"Module metadata '{module.MetadataFile}' contains unknown action '{unknown}' and was ignored.");
        }

        var scopes = module.Scopes.Select(scope =>
        {
            var code = NormalizeCode(scope.Code);
            scopeMetadata.TryGetValue(code, out var detail);
            return new OAuthScopeDeclaration(code, string.IsNullOrWhiteSpace(detail?.DisplayName) ? code : detail.DisplayName.Trim(), string.IsNullOrWhiteSpace(detail?.Description) ? null : detail.Description.Trim(), scope.Kind, scope.Parameter, scope.ParameterSource);
        }).ToArray();
        var actions = module.Actions.Select(action =>
        {
            var code = NormalizeCode(action.Code);
            actionMetadata.TryGetValue(code, out var detail);
            return action with
            {
                Code = code,
                DisplayName = string.IsNullOrWhiteSpace(detail?.DisplayName) ? action.DisplayName : detail.DisplayName.Trim(),
                Description = string.IsNullOrWhiteSpace(detail?.Description) ? action.Description : detail.Description.Trim()
            };
        }).ToArray();
        var normalizedModule = module with
        {
            OwnerFamily = NormalizeCode(module.OwnerFamily),
            ModuleCode = NormalizeCode(module.ModuleCode),
            ModuleVersion = module.ModuleVersion.Trim(),
            Actions = actions,
            ScopeActions = module.ScopeActions.Select(mapping => new KidaScopeActionDefinition(NormalizeCode(mapping.Scope), NormalizeCode(mapping.Action))).ToArray()
        };
        var displayName = string.IsNullOrWhiteSpace(metadataFile.ModuleDisplayName) ? normalizedModule.ModuleCode : metadataFile.ModuleDisplayName.Trim();
        var description = string.IsNullOrWhiteSpace(metadataFile.ModuleDescription) ? null : metadataFile.ModuleDescription.Trim();
        return new(new RegisterOAuthScopesRequest(normalizedModule.OwnerFamily, normalizedModule.ModuleCode, normalizedModule.ModuleVersion, displayName, description, normalizedAudience, scopes, options.OverwriteDescriptions), normalizedModule, warnings);
    }

    private static void ValidateCompiledCatalog(IKidaModuleCatalogProvider provider, KidaModuleCatalogDefinition module)
    {
        var scopeCodes = module.Scopes.Select(scope => NormalizeCode(scope.Code)).ToHashSet(StringComparer.Ordinal);
        var actionCodes = module.Actions.Select(action => NormalizeCode(action.Code)).ToHashSet(StringComparer.Ordinal);
        if (scopeCodes.Count != module.Scopes.Count || actionCodes.Count != module.Actions.Count)
        {
            throw new InvalidOperationException($"Module provider '{provider.GetType().FullName}' contains duplicate scope or action codes.");
        }

        foreach (var mapping in module.ScopeActions)
        {
            if (!scopeCodes.Contains(NormalizeCode(mapping.Scope)) || !actionCodes.Contains(NormalizeCode(mapping.Action)))
            {
                throw new InvalidOperationException($"Module provider '{provider.GetType().FullName}' maps an unknown scope or action.");
            }
        }
    }

    private static void LoadMetadata(string path, bool required, IDictionary<string, KidaScopeMetadata> scopes, IDictionary<string, KidaActionMetadata> actions, KidaScopeMetadataFile aggregate, ICollection<string> warnings)
    {
        if (!File.Exists(path))
        {
            if (required)
                warnings.Add($"Module metadata defaults were not found at '{path}'.");
            return;
        }

        try
        {
            var document = File.ReadAllText(path).FromJson<KidaScopeMetadataFile>();
            if (document is null)
            {
                warnings.Add($"Module metadata file '{path}' was empty and was ignored.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(document.ModuleDisplayName))
                aggregate.ModuleDisplayName = document.ModuleDisplayName;
            if (!string.IsNullOrWhiteSpace(document.ModuleDescription))
                aggregate.ModuleDescription = document.ModuleDescription;
            foreach (var item in document.Scopes)
            {
                var code = NormalizeCode(item.Code);
                if (code.Length == 0)
                {
                    warnings.Add($"Module metadata file '{path}' contains a scope without a code.");
                    continue;
                }

                scopes[code] = item;
            }

            foreach (var item in document.Actions)
            {
                var code = NormalizeCode(item.Code);
                if (code.Length == 0)
                {
                    warnings.Add($"Module metadata file '{path}' contains an action without a code.");
                    continue;
                }

                actions[code] = item;
            }
        }
        catch (Exception exception)when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            warnings.Add($"Module metadata file '{path}' could not be read: {exception.Message}");
        }
    }

    private static string NormalizeCode(string? value) => value?.Trim().Normalize().ToLowerInvariant() ?? string.Empty;
}
