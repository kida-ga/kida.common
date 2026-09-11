namespace Kida.Models;

public sealed record AppCatalogDetails(
    AppSummary App,
    IReadOnlyCollection<AppVersionInfo> Versions,
    IReadOnlyCollection<AppTrustInfo> TrustReferences);
