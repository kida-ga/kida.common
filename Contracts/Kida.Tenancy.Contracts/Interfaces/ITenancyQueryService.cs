using Kida.Models;

namespace Kida.Abstractions;

public interface ITenancyQueryService
{
    ValueTask<TenantPage> SearchAsync(
        TenantSearchRequest request,
        CancellationToken cancellationToken = default);
}
