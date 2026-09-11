namespace Kida.Models;

public sealed record EntitlementTenantSnapshot(
    Guid TenantId,
    IReadOnlyCollection<EntitlementSubscriptionInfo> Subscriptions,
    IReadOnlyCollection<EntitlementGrantInfo> Grants,
    IReadOnlyCollection<LicensePoolInfo> LicensePools,
    IReadOnlyCollection<LicenseAssignmentInfo> LicenseAssignments);
