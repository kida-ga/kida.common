using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Abstractions;
/// <summary>
/// Verifies the durable client-to-resource relationship managed by Kida Identity.
/// A machine token's audience identifies Kida as its recipient; it does not by
/// itself prove which product service audience the caller may publish or operate.
/// </summary>
public interface IIdentityClientResourceAuthorizationService
{
    ValueTask<bool> HasActiveResourceGrantAsync(Guid clientId, string audience, CancellationToken cancellationToken = default);
    ValueTask<bool> HasActiveHostedAudienceAsync(Guid clientId, string audience, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
    ValueTask<bool> HasAnyActiveHostedAudienceAsync(Guid clientId, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
    async ValueTask<bool> HasActiveResourceAuthorityAsync(Guid clientId, string audience, CancellationToken cancellationToken = default) =>
        await HasActiveHostedAudienceAsync(clientId, audience, cancellationToken).ConfigureAwait(false) ||
        await HasActiveResourceGrantAsync(clientId, audience, cancellationToken).ConfigureAwait(false);
    ValueTask<ClientResourceGrantSnapshot?> GetResourceGrantSnapshotAsync(Guid clientId, string audience, CancellationToken cancellationToken = default);
    ValueTask<ClientResourceGrantSnapshot?> GetAuthorizedResourceGrantSnapshotAsync(Guid callerClientId, Guid targetClientId, string audience, CancellationToken cancellationToken = default);
    async ValueTask<ClientResourceGrantRevision?> GetResourceGrantRevisionAsync(Guid clientId, string audience, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetResourceGrantSnapshotAsync(clientId, audience, cancellationToken).ConfigureAwait(false);
        return snapshot is null
            ? null
            : new(snapshot.ClientId, snapshot.Audience, snapshot.Status, snapshot.Version, snapshot.RevisionHash, snapshot.ModifiedAt);
    }
    async ValueTask<ClientResourceGrantRevision?> GetAuthorizedResourceGrantRevisionAsync(Guid callerClientId, Guid targetClientId, string audience, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetAuthorizedResourceGrantSnapshotAsync(callerClientId, targetClientId, audience, cancellationToken).ConfigureAwait(false);
        return snapshot is null
            ? null
            : new(snapshot.ClientId, snapshot.Audience, snapshot.Status, snapshot.Version, snapshot.RevisionHash, snapshot.ModifiedAt);
    }
    ValueTask<bool> IsReturnUriAllowedAsync(Guid clientId, string audience, string returnUri, CancellationToken cancellationToken = default);
    ValueTask<bool> IsReturnUriAllowedAsync(string clientIdentifier, string returnUri, CancellationToken cancellationToken = default);
}
