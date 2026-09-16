using Kida.Models;

namespace Kida.Service.Client;

public interface IKidaProductCatalogStatus
{
    EntitlementCatalogReceipt? Receipt { get; }
}
