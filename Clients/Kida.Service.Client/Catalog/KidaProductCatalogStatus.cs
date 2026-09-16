using Kida.Models;

namespace Kida.Service.Client;

internal sealed class KidaProductCatalogStatus : IKidaProductCatalogStatus
{
    public EntitlementCatalogReceipt? Receipt { get; private set; }

    internal void Set(EntitlementCatalogReceipt receipt) => Receipt = receipt;
}
