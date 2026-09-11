using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface IEntitlementService
{
    ValueTask<IFeedback<EntitlementCatalogReceipt>> RegisterCatalogAsync(RegisterEntitlementCatalogRequest request, CancellationToken cancellationToken = default);
    ValueTask<EntitlementDecision> EvaluateAsync(EvaluateEntitlementRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<UsageReceipt>> ReportUsageAsync(ReportUsageRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicenseLeaseReceipt>> AcquireLicenseAsync(AcquireLicenseRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<LicenseLeaseReceipt>> HeartbeatLicenseAsync(HeartbeatLicenseRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ReleaseLicenseAsync(ReleaseLicenseRequest request, CancellationToken cancellationToken = default);
}
