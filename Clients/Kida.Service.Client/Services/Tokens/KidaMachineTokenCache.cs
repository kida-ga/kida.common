using System.Net;
using Haley.Models;
using Kida.Constants;
using Kida.Models;

namespace Kida.Service.Client;

internal sealed class KidaMachineTokenCache(
    KidaClientTransport transport,
    string clientIdentifier,
    string clientSecret,
    TimeProvider timeProvider) : IDisposable
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private CachedClientToken? _cached;

    internal async ValueTask<KidaClientTokenLease> GetTokenAsync(
        CancellationToken cancellationToken = default)
    {
        var cached = Volatile.Read(ref _cached);
        if (cached is not null && IsUsable(cached)) return cached.Lease;

        await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cached = Volatile.Read(ref _cached);
            if (cached is not null && IsUsable(cached)) return cached.Lease;

            var response = await transport.Client
                .WithEndPoint("kida/identity/client-tokens")
                .WithBody(new RawBodyRequestContent(new MachineClientTokenRequest(clientIdentifier, clientSecret)))
                .AddHeader(KidaIdentityHeaders.ClientIdentifier, clientIdentifier)
                .AddCancellationToken(cancellationToken)
                .DoNotAuthenticate()
                .PostAsync()
                .ConfigureAwait(false);
            var token = await KidaResponseReader.ReadAsync<ClientTokenResult>(response).ConfigureAwait(false);
            if (token is not { Succeeded: true, AccessToken.Length: > 0, ExpiresAt: not null })
            {
                throw new KidaRequestException(
                    "Kida did not issue a client access token.",
                    HttpStatusCode.Unauthorized,
                    token.ErrorCode);
            }

            var grantedScopes = (token.Scope ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(scope => scope.ToLowerInvariant())
                .ToHashSet(StringComparer.Ordinal);
            var replacement = new CachedClientToken(
                new KidaClientTokenLease(token.AccessToken, token.ExpiresAt.Value, grantedScopes));
            Volatile.Write(ref _cached, replacement);
            return replacement.Lease;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    internal void Invalidate(string rejectedAccessToken)
    {
        if (string.IsNullOrWhiteSpace(rejectedAccessToken)) return;
        while (true)
        {
            var cached = Volatile.Read(ref _cached);
            if (cached is null ||
                !string.Equals(cached.Lease.AccessToken, rejectedAccessToken, StringComparison.Ordinal)) return;
            if (ReferenceEquals(Interlocked.CompareExchange(ref _cached, null, cached), cached)) return;
        }
    }

    private bool IsUsable(CachedClientToken token) =>
        token.Lease.ExpiresAt > timeProvider.GetUtcNow().AddSeconds(60);

    public void Dispose() => _refreshLock.Dispose();
}
