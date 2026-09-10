using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface IIdentityDelegatedClientService
{
    ValueTask<IFeedback<OAuthClientRegistrationResult>> RegisterManagedClientAsync(
        Guid managerClientId,
        RegisterManagedOAuthClientRequest request,
        CancellationToken cancellationToken = default);
}
