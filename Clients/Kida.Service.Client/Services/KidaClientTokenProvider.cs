using Kida.Abstractions;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaClientTokenProvider : IKidaClientTokenProvider, IDisposable
{
    private readonly KidaMachineTokenCache _tokens;

    public KidaClientTokenProvider(
        KidaClientTransport transport,
        IOptions<KidaClientOptions> options,
        TimeProvider timeProvider)
    {
        var value = options.Value;
        _tokens = new KidaMachineTokenCache(
            transport,
            value.ClientIdentifier,
            value.ClientSecret,
            timeProvider);
    }

    public ValueTask<KidaClientTokenLease> GetTokenAsync(
        CancellationToken cancellationToken = default) =>
        _tokens.GetTokenAsync(cancellationToken);

    public async ValueTask<string> GetAccessTokenAsync(
        CancellationToken cancellationToken = default) =>
        (await GetTokenAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

    public void Invalidate(string rejectedAccessToken) =>
        _tokens.Invalidate(rejectedAccessToken);

    public void Dispose() => _tokens.Dispose();
}
