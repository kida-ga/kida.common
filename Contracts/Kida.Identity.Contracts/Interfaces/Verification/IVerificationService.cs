using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IVerificationService
{
    ValueTask<IFeedback<VerificationChallengeReceipt>> CreateChallengeAsync(CreateVerificationChallengeRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<VerificationGrantReceipt>> VerifyChallengeAsync(VerifyChallengeRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ConsumeGrantAsync(ConsumeVerificationGrantRequest request, CancellationToken cancellationToken = default);
}
