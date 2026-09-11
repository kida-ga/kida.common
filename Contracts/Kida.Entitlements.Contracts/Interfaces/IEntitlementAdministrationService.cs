using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface IEntitlementAdministrationService
{
    ValueTask<IReadOnlyCollection<EntitlementProductInfo>> ListProductsAsync(CancellationToken cancellationToken = default);
    ValueTask<EntitlementProductCatalog?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<EntitlementPlanInfo>> CreatePlanAsync(CreateEntitlementPlanRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<EntitlementSubscriptionInfo>> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<EntitlementGrantInfo>> GrantAsync(GrantEntitlementRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RevokeGrantAsync(Guid grantId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicensePoolInfo>> CreateLicensePoolAsync(CreateLicensePoolRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicenseAssignmentInfo>> AssignNamedLicenseAsync(AssignNamedLicenseRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ReleaseNamedLicenseAsync(Guid tenantId, Guid assignmentId, CancellationToken cancellationToken = default);
    ValueTask<EntitlementTenantSnapshot> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
