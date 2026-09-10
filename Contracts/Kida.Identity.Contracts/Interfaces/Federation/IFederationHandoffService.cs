using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IFederationHandoffService
{
    ValueTask<IFeedback<FederatedIdentityResult>> RedeemAsync(RedeemFederationHandoffRequest request, CancellationToken cancellationToken = default);
}
