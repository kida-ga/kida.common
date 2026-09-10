namespace Kida.Models;

public sealed record CompletePasswordResetRequest(
    Guid GrantId,
    Guid ClientId,
    string Resource,
    string NewPassword,
    string? ReturnUri = null,
    string? State = null);
