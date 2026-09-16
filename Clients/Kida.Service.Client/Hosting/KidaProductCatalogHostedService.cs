using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaProductCatalogHostedService(
    IKidaProductCatalogProvider provider,
    IKidaClient client,
    KidaProductCatalogStatus status,
    IOptions<KidaProductCatalogOptions> options) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.RegisterOnStartup) return;
        var catalog = await provider.GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        var receipt = await client.RegisterEntitlementCatalogAsync(catalog, cancellationToken).ConfigureAwait(false);
        status.Set(receipt);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
