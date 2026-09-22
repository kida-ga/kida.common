namespace Kida.Models;

public sealed record ClientPasswordResetVerificationRequest(
    Guid ChallengeId,
    Guid ClientId,
    string Resource,
    string Code,
    string? ReturnUri = null,
    string? State = null);
