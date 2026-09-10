using Kida.Models;

namespace Kida.Service.Client;

public interface IKidaIntegrationClientManager
{
    ValueTask<OAuthClientRegistrationResult> RegisterClientAsync(
        RegisterManagedOAuthClientRequest request,
        CancellationToken cancellationToken = default);
}
