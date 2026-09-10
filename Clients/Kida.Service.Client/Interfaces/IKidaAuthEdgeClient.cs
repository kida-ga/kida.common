using Kida.Models;

namespace Kida.Service.Client;

/// <summary>
/// Narrow transport used by a product-hosted Kida authentication facade.
/// Implementations bind every request to the configured product client and audience.
/// </summary>
public interface IKidaAuthEdgeClient
{
    ValueTask<ClientTokenResult> IssueProductClientTokenAsync(
        string clientIdentifier,
        string clientSecret,
        CancellationToken cancellationToken = default);

    ValueTask<PasswordChangeReceipt?> ChangePasswordAtEdgeAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default);

    ValueTask<PasswordResetGrantReceipt> VerifyPasswordResetAsync(
        Guid challengeId,
        string code,
        string? returnUri = null,
        string? state = null,
        CancellationToken cancellationToken = default);

    ValueTask<PasswordResetCompletionReceipt> CompletePasswordResetAsync(
        Guid grantId,
        string newPassword,
        string? returnUri = null,
        string? state = null,
        CancellationToken cancellationToken = default);

    ValueTask<SamlAuthenticationStart> BeginSamlAuthenticationAsync(
        string providerCode,
        string returnUri,
        string state,
        string codeChallenge,
        CancellationToken cancellationToken = default);

    ValueTask<SamlAuthenticationHandoff> CompleteSamlAuthenticationAsync(
        string samlResponse,
        string relayState,
        CancellationToken cancellationToken = default);

    ValueTask<TotpEnrollmentDetails> InspectTotpEnrollmentAsync(
        string ticket,
        CancellationToken cancellationToken = default);

    ValueTask<TotpEnrollmentCompletion> ConfirmTotpEnrollmentAsync(
        string ticket,
        string code,
        CancellationToken cancellationToken = default);
}
