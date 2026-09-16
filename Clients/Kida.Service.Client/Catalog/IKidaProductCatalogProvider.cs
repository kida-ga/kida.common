using Kida.Models;

namespace Kida.Service.Client;

public interface IKidaProductCatalogProvider
{
    ValueTask<RegisterEntitlementCatalogRequest> GetCatalogAsync(CancellationToken cancellationToken = default);
}
