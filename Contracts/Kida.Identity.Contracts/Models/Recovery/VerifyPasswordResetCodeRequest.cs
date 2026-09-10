namespace Kida.Models;

public sealed record VerifyPasswordResetCodeRequest(
    Guid ChallengeId,
    Guid ClientId,
    string Resource,
    string Code,
    string? ReturnUri = null,
    string? State = null);
