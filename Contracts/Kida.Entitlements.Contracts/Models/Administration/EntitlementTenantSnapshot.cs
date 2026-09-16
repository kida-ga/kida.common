namespace Kida.Models;

public sealed record EntitlementTenantSnapshot(
    Guid TenantId,
    IReadOnlyCollection<EntitlementSubscriptionInfo> Subscriptions,
    IReadOnlyCollection<LicensePoolInfo> LicensePools,
    IReadOnlyCollection<LicenseAssignmentInfo> LicenseAssignments);
