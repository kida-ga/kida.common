using Haley.Abstractions;

namespace Kida.Abstractions;

public interface IPasswordlessAuthenticationService
{
    ValueTask<IFeedback<PasswordlessAuthenticationInitiationResult>> BeginAsync(
        BeginPasswordlessAuthenticationRequest request,
        CancellationToken cancellationToken = default);

    ValueTask<IFeedback<PasswordlessAuthenticationResult>> CompleteAsync(
        CompletePasswordlessAuthenticationRequest request,
        CancellationToken cancellationToken = default);
}
