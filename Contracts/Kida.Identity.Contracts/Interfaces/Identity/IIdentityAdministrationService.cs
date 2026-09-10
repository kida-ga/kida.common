using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityAdministrationService
{
    ValueTask<IReadOnlyCollection<UserIdentity>> ListUsersAsync(UserSearchRequest request, CancellationToken cancellationToken = default);
    ValueTask<UserIdentityPage> ListUserPageAsync(UserPageRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ChangeUserStatusAsync(ChangeUserStatusRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RestoreUserAsync(Guid userId, string reasonCode, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> PermanentlyDeleteUserAsync(Guid userId, string reasonCode, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RecordLoginEmailContactAsync(Guid userId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> OverrideEmailVerificationAsync(Guid userId, Guid contactId, string reasonCode, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> ResetUserPasswordAsync(ResetUserPasswordRequest request, CancellationToken cancellationToken = default);
    ValueTask<UserSessionPage> ListUserSessionsAsync(UserSessionSearchRequest request, CancellationToken cancellationToken = default);
    ValueTask<UserLoginAttemptPage> ListUserLoginAttemptsAsync(UserLoginAttemptSearchRequest request, CancellationToken cancellationToken = default);
}
