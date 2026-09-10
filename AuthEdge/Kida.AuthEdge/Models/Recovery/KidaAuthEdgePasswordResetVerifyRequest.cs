namespace Kida.Models;

public sealed record KidaAuthEdgePasswordResetVerifyRequest(
    Guid ChallengeId,
    string Code,
    string? ReturnUri = null,
    string? State = null);
