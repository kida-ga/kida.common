using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IFederatedIdentityService
{
    ValueTask<IFeedback<FederatedIdentityResult>> ExchangeAsync(ExchangeFederatedIdentityRequest request, CancellationToken cancellationToken = default);
}
