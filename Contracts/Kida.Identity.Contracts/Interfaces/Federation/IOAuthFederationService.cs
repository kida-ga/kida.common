using Haley.Abstractions;
using Haley.Models;

namespace Kida.Abstractions;

public interface IOAuthFederationService
{
    ValueTask<IFeedback<FederatedIdentityResult>> RedeemAsync(RedeemFederationHandoffRequest request, CancellationToken cancellationToken = default);
}
