using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityPublicKeyProvider
{
    ValueTask<IdentityJsonWebKey> GetJsonWebKeyAsync(CancellationToken cancellationToken);
    async ValueTask<IReadOnlyCollection<IdentityJsonWebKey>> GetJsonWebKeysAsync(CancellationToken cancellationToken) => [await GetJsonWebKeyAsync(cancellationToken).ConfigureAwait(false)];
}
