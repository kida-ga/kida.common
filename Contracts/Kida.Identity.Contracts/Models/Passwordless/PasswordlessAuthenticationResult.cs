namespace Kida.Models;

public sealed record PasswordlessAuthenticationResult(
    AuthenticationResult Session,
    string? ReturnUri = null,
    string? State = null);
