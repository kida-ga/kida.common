namespace Kida.Abstractions;
public interface IIdentityScopeCatalogService
{
    ValueTask<OAuthScopeRegistrationResult> RegisterScopesAsync(RegisterOAuthScopesRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<OAuthScopeInfo>> ListScopesAsync(CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<OAuthModuleInfo>> ListModulesAsync(CancellationToken cancellationToken = default);
    ValueTask<OAuthScopeResolution> ResolveScopesAsync(string audience, IReadOnlyCollection<string> scopes, CancellationToken cancellationToken = default);
    ValueTask<bool> IsModuleBoundToResourceAsync(string audience, string moduleCode, string? moduleVersion = null, CancellationToken cancellationToken = default);
}
