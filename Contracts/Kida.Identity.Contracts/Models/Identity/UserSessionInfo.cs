using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserSessionInfo(Guid SessionId, Guid UserId, Guid? ClientId, string? Resource, string Status, DateTimeOffset AuthenticatedAt, DateTimeOffset LastSeenAt, DateTimeOffset ExpiresAt, DateTimeOffset? EndedAt);
