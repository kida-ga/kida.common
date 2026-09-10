using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaClientScopeCatalogHostedService(
    IOptions<KidaScopeCatalogOptions> options,
    IEnumerable<IKidaModuleCatalogProvider> providers,
    IKidaClient client,
    ILogger<KidaClientScopeCatalogHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.RegisterOnStartup) return;

        var audiences = options.Value.GetAudiences();
        if (audiences.Count == 0)
        {
            throw new InvalidOperationException(
                $"Configure at least one service audience under '{KidaScopeCatalogConfiguration.SectionName}:Audience' or ':Audiences'.");
        }

        foreach (var provider in providers)
        {
            foreach (var audience in audiences)
            {
                var composition = KidaScopeCatalogComposer.Compose(provider, audience, options.Value);
                foreach (var warning in composition.Warnings)
                {
                    logger.LogWarning("{ScopeCatalogWarning}", warning);
                }

                var request = composition.ScopeRequest;
                var result = await client.RegisterScopesAsync(request, cancellationToken).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Kida rejected scope registration for module '{request.ModuleCode}' version " +
                        $"'{request.ModuleVersion}' and audience '{request.Audience}'. Error={result.ErrorCode}");
                }

                logger.LogInformation(
                    "Registered {ScopeCount} Kida scopes for module {ModuleCode} version {ModuleVersion} at service audience {Audience}; omitted catalog entries were retained.",
                    result.RegisteredCount,
                    composition.Module.ModuleCode,
                    composition.Module.ModuleVersion,
                    request.Audience);

                var module = composition.Module;
                if (module.Actions.Count > 0)
                {
                    var accessResult = await client.RegisterModuleAsync(
                        new(
                            request.Audience,
                            module.ModuleCode,
                            request.ModuleDisplayName,
                            module.OwnerFamily,
                            module.ModuleVersion,
                            module.Actions.Select(action => new Kida.Models.RegisterActionRequest(
                                action.Code,
                                action.DisplayName,
                                action.Description,
                                action.RiskLevel)).ToArray(),
                            module.ScopeActions.Select(mapping => new Kida.Models.RegisterScopeActionRequest(
                                mapping.Scope,
                                mapping.Action)).ToArray()),
                        cancellationToken).ConfigureAwait(false);
                    if (!accessResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Kida rejected the action catalog for module '{module.ModuleCode}' version " +
                            $"'{module.ModuleVersion}' at service audience '{request.Audience}'. " +
                            $"Error={accessResult.ErrorCode}");
                    }
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
