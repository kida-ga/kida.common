namespace Kida.Models;

public sealed record BeginPasswordResetRequest(
    Guid ClientId,
    string Resource,
    string Channel,
    string Destination,
    string? ReturnUri = null,
    string? State = null);
