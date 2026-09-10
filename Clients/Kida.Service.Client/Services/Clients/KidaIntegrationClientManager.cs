using System.Net;
using Haley.Abstractions;
using Haley.Models;
using Haley.Utils;
using Kida.Constants;
using Kida.Models;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaIntegrationClientManager : IKidaIntegrationClientManager, IDisposable
{
    private readonly KidaClientTransport _transport;
    private readonly KidaMachineTokenCache _tokens;

    public KidaIntegrationClientManager(
        KidaClientTransport transport,
        IOptions<KidaIntegrationManagementOptions> options,
        TimeProvider timeProvider)
    {
        _transport = transport;
        var value = options.Value;
        _tokens = new KidaMachineTokenCache(
            transport,
            value.ClientIdentifier,
            value.ClientSecret,
            timeProvider);
    }

    public async ValueTask<OAuthClientRegistrationResult> RegisterClientAsync(
        RegisterManagedOAuthClientRequest request,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var token = await _tokens.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!token.Scopes.Contains(KidaIdentityScopes.ClientsManage))
            {
                throw new KidaRequestException(
                    $"Kida issued the integration-manager token without required scope '{KidaIdentityScopes.ClientsManage}'.",
                    HttpStatusCode.Forbidden,
                    IdentityErrorCodes.InvalidClientScope);
            }

            var response = await _transport.Client
                .WithEndPoint("kida/identity/integration-clients")
                .WithBody(new RawBodyRequestContent(request))
                .AddCancellationToken(cancellationToken)
                .SetAuthenticator(new TokenAuthProvider().SetToken(token.AccessToken))
                .PostAsync()
                .ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && attempt == 0)
            {
                response.OriginalResponse?.Dispose();
                _tokens.Invalidate(token.AccessToken);
                continue;
            }

            return await KidaResponseReader.ReadAsync<OAuthClientRegistrationResult>(response)
                .ConfigureAwait(false);
        }

        throw new InvalidOperationException("The Kida request retry loop ended unexpectedly.");
    }

    public void Dispose() => _tokens.Dispose();
}
