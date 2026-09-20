using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface IEntitlementAdministrationService
{
    ValueTask<IReadOnlyCollection<EntitlementProductInfo>> ListProductsAsync(CancellationToken cancellationToken = default);
    ValueTask<EntitlementProductCatalog?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    ValueTask<EntitlementProductCatalog?> GetProductAsync(string audience, string productCode, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<EntitlementPlanInfo>> CreatePlanAsync(CreateEntitlementPlanRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<EntitlementPlanInfo>> ChangePlanStatusAsync(Guid planId, EntitlementStatus status, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> DeletePlanAsync(Guid planId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<EntitlementSubscriptionInfo>> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<EntitlementSubscriptionInfo>> GetTenantSubscriptionsAsync(Guid tenantId, string audience, string productCode, CancellationToken cancellationToken = default);
    ValueTask<EntitlementSubscriptionPage> GetProductSubscriptionHistoryAsync(Guid tenantId, string audience, string productCode, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    ValueTask<EntitlementSubscriptionPage> GetTenantSubscriptionHistoryAsync(Guid tenantId, Guid? productId = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> DeleteSubscriptionAsync(Guid tenantId, Guid subscriptionId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicensePoolInfo>> CreateLicensePoolAsync(CreateLicensePoolRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicensePoolInfo>> UpdateLicensePoolAsync(Guid tenantId, Guid poolId, UpdateLicensePoolRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> DeleteLicensePoolAsync(Guid tenantId, Guid poolId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicenseAssignmentInfo>> AssignNamedLicenseAsync(AssignNamedLicenseRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ReleaseNamedLicenseAsync(Guid tenantId, Guid assignmentId, CancellationToken cancellationToken = default);
    ValueTask<EntitlementTenantSnapshot> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
