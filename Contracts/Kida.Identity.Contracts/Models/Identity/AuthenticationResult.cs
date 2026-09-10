using Haley.Abstractions;

namespace Kida.Models;
public sealed record AuthenticationResult(bool Succeeded, UserIdentity? Identity = null, Guid? SessionId = null, string? AccessToken = null, DateTimeOffset? AccessTokenExpiresAt = null, string? RefreshToken = null, DateTimeOffset? RefreshTokenExpiresAt = null, bool PasswordChangeRequired = false, string? ErrorCode = null, string? Resource = null, IReadOnlyCollection<string>? Scopes = null)
{
    public static AuthenticationResult Failure(string errorCode) => new(false, ErrorCode: errorCode);
}
