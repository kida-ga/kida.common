using Haley.Abstractions;
using Kida.Constants;
using Microsoft.Extensions.Options;
namespace Kida.Service.Client;
internal sealed class IdentityKidaAuthentication(IKidaClientTokenProvider tokens,
    IOptions<KidaClientOptions> options) : global::Haley.Abstractions.IIdentityRemoteAuthentication
{
    public async ValueTask PrepareAsync(IRequest request, string operation, CancellationToken cancellationToken)
    {
        _ = KidaIdentityFoundationScopes.ForOperation(operation);
        request.AddHeader("Authorization", "Bearer " + await tokens.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false));
        request.AddHeader(KidaIdentityHeaders.ClientIdentifier, options.Value.ClientIdentifier);
        request.AddHeader(KidaIdentityFoundationScopes.ResourceHeader, options.Value.UserAudience);
    }
}
