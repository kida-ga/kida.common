namespace Kida.Models;

public sealed record EntitlementProductCatalog(
    EntitlementProductInfo Product,
    IReadOnlyCollection<EntitlementProductReleaseInfo> Releases,
    IReadOnlyCollection<EntitlementFeatureInfo> Features,
    IReadOnlyCollection<EntitlementMeterInfo> Meters,
    IReadOnlyCollection<EntitlementPlanInfo> Plans);
