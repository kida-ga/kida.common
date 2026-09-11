namespace Kida.Models;

public sealed record EntitlementProductCatalog(
    EntitlementProductInfo Product,
    IReadOnlyCollection<EntitlementFeatureInfo> Features,
    IReadOnlyCollection<EntitlementMeterInfo> Meters,
    IReadOnlyCollection<EntitlementPlanInfo> Plans);
