using Kida.Models;

namespace Kida.Abstractions;

public interface ITenancyQueryService
{
    ValueTask<TenantSummary?> GetAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    ValueTask<TenantPage> SearchAsync(
        TenantSearchRequest request,
        CancellationToken cancellationToken = default);
}
