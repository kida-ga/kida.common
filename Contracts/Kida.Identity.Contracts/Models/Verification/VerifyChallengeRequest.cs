using Haley.Abstractions;

namespace Kida.Models;
public sealed record VerifyChallengeRequest(Guid ChallengeId, string? Code = null, string? ActivationToken = null, string? IpAddress = null, string? UserAgent = null);
