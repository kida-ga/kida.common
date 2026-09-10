using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Haley.Abstractions;
using Haley.Rest;
using Haley.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kida.ResourceServer;
internal sealed class KidaJwksCache(KidaJwksTransport transport, IOptions<KidaResourceServerOptions> options, TimeProvider timeProvider) : IDisposable
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private CachedKeys? _cached;
    public async ValueTask<IReadOnlyCollection<SecurityKey>> GetKeysAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        var cached = Volatile.Read(ref _cached);
        if (!forceRefresh && IsUsable(cached))
            return cached!.Keys;
        await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cached = Volatile.Read(ref _cached);
            if (!forceRefresh && IsUsable(cached))
                return cached!.Keys;
            var response = await transport.Client.WithEndPoint(transport.Endpoint).AddCancellationToken(cancellationToken).DoNotAuthenticate().GetAsync().ConfigureAwait(false);
            try
            {
                var content = (await response.AsStringResponseAsync().ConfigureAwait(false)).Content;
                if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrWhiteSpace(content))
                {
                    throw new InvalidOperationException($"Kida JWKS discovery failed with HTTP {(int)response.StatusCode}.");
                }

                var document = content.FromJson<KidaJsonWebKeySet>();
                var keys = document?.Keys?.Where(IsSupported).Select(ToSecurityKey).ToArray() ?? [];
                if (keys.Length == 0 || keys.Select(key => key.KeyId).Distinct(StringComparer.Ordinal).Count() != keys.Length)
                {
                    throw new InvalidOperationException("Kida JWKS did not contain a valid unique RS256 signing-key set.");
                }

                var lifetime = Math.Clamp(options.Value.JwksRefreshSeconds, 30, 86_400);
                var replacement = new CachedKeys(keys, timeProvider.GetUtcNow().AddSeconds(lifetime));
                Volatile.Write(ref _cached, replacement);
                return replacement.Keys;
            }
            finally
            {
                response.OriginalResponse?.Dispose();
            }
        }
        catch (Exception exception)when (exception is not OperationCanceledException && cached is not null && cached.Keys.Count > 0)
        {
            // A transient discovery failure must not discard keys that were already
            // trusted. Unknown kid resolution still fails closed in the resolver.
            return cached.Keys;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private bool IsUsable(CachedKeys? cached) => cached is not null && cached.RefreshAt > timeProvider.GetUtcNow();
    private static bool IsSupported(KidaJsonWebKey key) => string.Equals(key.Kty, "RSA", StringComparison.Ordinal) && string.Equals(key.Use, "sig", StringComparison.Ordinal) && string.Equals(key.Alg, "RS256", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(key.Kid) && !string.IsNullOrWhiteSpace(key.N) && !string.IsNullOrWhiteSpace(key.E);
    private static SecurityKey ToSecurityKey(KidaJsonWebKey key)
    {
        try
        {
            return new RsaSecurityKey(new RSAParameters { Modulus = key.N.SafeBase64Decode(), Exponent = key.E.SafeBase64Decode() })
            {
                KeyId = key.Kid
            };
        }
        catch (Exception exception)when (exception is FormatException or CryptographicException)
        {
            throw new InvalidOperationException($"Kida JWKS key '{key.Kid}' is malformed.", exception);
        }
    }

    public void Dispose() => _refreshLock.Dispose();
}
