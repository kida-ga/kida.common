using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Abstractions;
public interface IIdentityClientAdministrationService
{
    ValueTask<IFeedback<OAuthAudienceInfo>> RegisterAudienceAsync(RegisterOAuthAudienceRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<OAuthAudienceInfo>> ListAudiencesAsync(CancellationToken cancellationToken = default);
    ValueTask<IFeedback> DeleteAudienceAsync(string audience, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<OAuthClientRegistrationResult>> RegisterClientAsync(RegisterOAuthClientRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<OAuthClientInfo>> ListClientsAsync(CancellationToken cancellationToken = default);
    ValueTask<OAuthClientPage> ListClientPageAsync(OAuthClientPageRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<OAuthClientSecretResult>> RotateClientSecretAsync(Guid clientId, RotateOAuthClientSecretRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> UpdateClientResourceGrantsAsync(Guid clientId, UpdateOAuthClientResourceGrantsRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> UpdateClientAsync(Guid clientId, UpdateOAuthClientRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RetireClientAsync(Guid clientId, string reasonCode, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<OAuthClientRestoreResult>> RestoreClientAsync(Guid clientId, string reasonCode, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> PermanentlyDeleteClientAsync(Guid clientId, string reasonCode, CancellationToken cancellationToken = default);
}
