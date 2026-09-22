using Haley.Models;
using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityOnboardingService
{
    ValueTask<IFeedback<IdentityInvitationReceipt>> InviteAsync(InviteIdentityRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<VerificationGrantReceipt>> VerifyInvitationAsync(VerifyIdentityInvitationRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<UserIdentity>> CompleteEnrollmentAsync(CompleteIdentityEnrollmentRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<ProvisionedUser>> BootstrapAsync(BootstrapIdentityRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<IdentityInvitationReceipt>> MigrateLegacyAsync(MigrateLegacyIdentityRequest request, CancellationToken cancellationToken = default);
}
