using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface IAppsService
{
    ValueTask<IFeedback<AppCatalogReceipt>> RegisterCatalogAsync(RegisterAppCatalogRequest request, CancellationToken cancellationToken = default);
    ValueTask<AppPage> SearchAsync(string? query, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<AppInstallationInfo>> InstallAsync(InstallAppRequest request, CancellationToken cancellationToken = default);
    ValueTask<AppComposition> ComposeAsync(Guid tenantId, Guid? projectId, string hostAudience, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ReportHealthAsync(ReportAppHealthRequest request, CancellationToken cancellationToken = default);
}
