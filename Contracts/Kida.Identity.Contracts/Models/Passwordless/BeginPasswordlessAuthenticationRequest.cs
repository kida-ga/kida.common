namespace Kida.Models;

public sealed record BeginPasswordlessAuthenticationRequest(
    Guid ClientId,
    string Resource,
    string Email,
    string Method,
    string? ReturnUri = null,
    string? State = null);
