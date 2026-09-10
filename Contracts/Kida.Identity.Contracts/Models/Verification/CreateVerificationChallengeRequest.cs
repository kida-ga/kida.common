using Haley.Abstractions;

namespace Kida.Models;
public sealed record CreateVerificationChallengeRequest(Guid ClientId, string PolicyCode, string Purpose, string SubjectType, Guid? SubjectId, string Destination, string Context, Guid? TenantId = null);
