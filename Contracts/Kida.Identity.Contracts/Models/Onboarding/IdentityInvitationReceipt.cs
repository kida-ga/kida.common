using Haley.Models;
using Haley.Abstractions;

namespace Kida.Models;
/// <summary>
/// One-time delivery material returned only to the authorized backend. Kida
/// persists verifiers, never these plaintext values. The client owns delivery.
/// </summary>
public sealed record IdentityInvitationReceipt(UserIdentity Identity, Guid? ChallengeId, string? OneTimeCode, string? ActivationToken, DateTimeOffset? ExpiresAt, DateTimeOffset? ResendAllowedAt, EmailVerificationMode VerificationMode, bool Created, string Context);
