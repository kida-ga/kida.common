using Haley.Abstractions;

namespace Kida.Abstractions;

public interface IIdentityProviderTenantService
{
    ValueTask<IReadOnlyCollection<ProviderTenantBinding>> ListAsync(CancellationToken cancellationToken = default);
    ValueTask<IFeedback> SetAsync(ProviderTenantBinding binding, CancellationToken cancellationToken = default);
}
