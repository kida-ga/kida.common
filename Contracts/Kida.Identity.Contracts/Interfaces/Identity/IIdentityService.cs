using Haley.Models;
using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityService
{
    ValueTask<IFeedback<UserIdentity>> CreateLocalUserAsync(CreateLocalUserRequest request, CancellationToken cancellationToken = default);
    ValueTask<AuthenticationResult> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default);
    ValueTask<AuthenticationResult> RefreshSessionAsync(RefreshSessionRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    ValueTask<UserProfile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<UserProfile>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RevokeSessionAsync(RevokeSessionRequest request, CancellationToken cancellationToken = default);
    ValueTask<UserIdentity?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
