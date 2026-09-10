using Haley.Abstractions;

namespace Kida.Models;
public sealed record VerifyIdentityInvitationRequest(Guid ChallengeId, string? Code = null, string? ActivationToken = null);
