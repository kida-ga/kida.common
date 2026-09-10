namespace Kida.Models;

public sealed record UserLoginAttemptInfo(
    long AttemptId,
    Guid UserId,
    Guid? ClientId,
    string Outcome,
    string? ReasonCode,
    DateTimeOffset OccurredAt);
