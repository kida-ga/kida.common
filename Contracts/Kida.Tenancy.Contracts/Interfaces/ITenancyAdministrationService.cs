using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface ITenancyAdministrationService
{
    ValueTask<IFeedback<TenantSummary>> CreateAsync(
        CreateTenantRequest request,
        CancellationToken cancellationToken = default);

    ValueTask<IFeedback<TenantSummary>> UpdateAsync(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default);

    ValueTask<IFeedback> PermanentlyDeleteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
