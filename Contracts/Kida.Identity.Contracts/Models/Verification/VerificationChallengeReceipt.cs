using Haley.Abstractions;

namespace Kida.Models;
public sealed record VerificationChallengeReceipt(Guid ChallengeId, DateTimeOffset ExpiresAt, DateTimeOffset ResendAllowedAt, string? OneTimeCode = null, string? ActivationToken = null);
