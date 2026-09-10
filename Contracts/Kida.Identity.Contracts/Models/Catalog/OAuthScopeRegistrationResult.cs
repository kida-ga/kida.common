namespace Kida.Models;
public sealed record OAuthScopeRegistrationResult(bool Succeeded, int RegisteredCount, string? ErrorCode = null)
{
    public static OAuthScopeRegistrationResult Failure(string errorCode) => new(false, 0, errorCode);
}
