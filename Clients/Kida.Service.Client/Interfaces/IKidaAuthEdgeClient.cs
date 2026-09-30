using Haley.Abstractions;
using Haley.Models;
using Kida.Models;

namespace Kida.Service.Client;

/// <summary>
/// Narrow transport used by a product-hosted Kida authentication facade.
/// Implementations bind every request to the configured product client and audience.
/// </summary>
public interface IKidaAuthEdgeClient
{
    ValueTask<IReadOnlyCollection<ProviderDiscovery>> DiscoverIdentityProvidersAsync(ProviderDiscoveryRequest request, CancellationToken cancellationToken = default) =>
        ValueTask.FromException<IReadOnlyCollection<ProviderDiscovery>>(new NotSupportedException());
    ValueTask<FederationStart> BeginFederationAsync(BeginFederationRequest request, CancellationToken cancellationToken = default) =>
        ValueTask.FromException<FederationStart>(new NotSupportedException());
    ValueTask<PasswordlessAuthenticationInitiationResult> BeginPasswordlessAuthenticationAsync(
        BeginPasswordlessAuthenticationRequest request,
        CancellationToken cancellationToken = default) =>
        ValueTask.FromException<PasswordlessAuthenticationInitiationResult>(new NotSupportedException());

    ValueTask<PasswordlessAuthenticationResult> CompletePasswordlessAuthenticationAsync(
        CompletePasswordlessAuthenticationRequest request,
        CancellationToken cancellationToken = default) =>
        ValueTask.FromException<PasswordlessAuthenticationResult>(new NotSupportedException());

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

    ValueTask<FederationStart> BeginFederationAsync(
        string providerCode,
        string returnUri,
        string state,
        string codeChallenge,
        CancellationToken cancellationToken = default);

    ValueTask<FederationHandoff> CompleteSamlAuthenticationAsync(
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
