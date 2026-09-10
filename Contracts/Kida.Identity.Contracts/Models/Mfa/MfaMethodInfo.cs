using Haley.Abstractions;

namespace Kida.Models;
public sealed record MfaMethodInfo(Guid MethodId, Guid UserId, MfaKind Kind, string? Label, string Status, DateTimeOffset CreatedAt, DateTimeOffset? VerifiedAt, DateTimeOffset? LastUsedAt);
