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
    KidaScopeCatalogStatus status,
    ILogger<KidaClientScopeCatalogHostedService> logger) : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _stopping = new();
    private Task? _worker;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.RegisterOnStartup) return Task.CompletedTask;

        _worker = RunAsync(_stopping.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _stopping.CancelAsync().ConfigureAwait(false);
        if (_worker is null) return;

        try
        {
            await _worker.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || _stopping.IsCancellationRequested)
        {
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var retryDelay = TimeSpan.FromSeconds(5);
        while (!cancellationToken.IsCancellationRequested)
        {
            var attemptedUtc = DateTimeOffset.UtcNow;
            status.MarkAttempt(attemptedUtc);
            try
            {
                await RegisterAsync(cancellationToken).ConfigureAwait(false);
                status.MarkAvailable(DateTimeOffset.UtcNow);
                return;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                status.MarkUnavailable(attemptedUtc, exception);
                logger.LogError(
                    exception,
                    "Kida catalog registration is unavailable. The host remains live and registration will be retried in {RetrySeconds} seconds.",
                    retryDelay.TotalSeconds);
            }

            try
            {
                await Task.Delay(retryDelay, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, 60));
        }
    }

    private async Task RegisterAsync(CancellationToken cancellationToken)
    {

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

    public void Dispose() => _stopping.Dispose();
}
