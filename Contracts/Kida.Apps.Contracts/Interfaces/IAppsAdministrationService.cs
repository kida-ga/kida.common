using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface IAppsAdministrationService
{
    ValueTask<IFeedback<AppTrustInfo>> RegisterPublisherAsync(RegisterAppPublisherRequest request, CancellationToken cancellationToken = default);
    ValueTask<AppPage> SearchAsync(string? query, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    ValueTask<AppCatalogDetails?> GetAsync(Guid appId, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<AppInstallationInfo>> ListInstallationsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ChangeAppStatusAsync(Guid appId, string status, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ChangeInstallationStatusAsync(Guid installationId, string status, CancellationToken cancellationToken = default);
}
