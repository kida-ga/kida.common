namespace Kida.Service.Client;

/// <summary>
/// Exposes the last known state of this host's Kida catalog registration.
/// </summary>
public interface IKidaScopeCatalogStatus
{
    KidaScopeCatalogStatusSnapshot GetSnapshot();
}
