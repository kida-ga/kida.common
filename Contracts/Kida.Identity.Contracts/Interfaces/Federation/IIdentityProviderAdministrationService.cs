using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityProviderAdministrationService
{
    ValueTask<IReadOnlyCollection<IdentityProviderInfo>> ListProvidersAsync(CancellationToken cancellationToken = default);
    ValueTask<IFeedback<IdentityProviderInfo>> UpsertProviderAsync(Guid? providerId, UpsertIdentityProviderRequest request, CancellationToken cancellationToken = default);
}
