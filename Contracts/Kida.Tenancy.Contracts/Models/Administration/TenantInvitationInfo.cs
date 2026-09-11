namespace Kida.Models;

public sealed record TenantInvitationInfo(Guid InvitationId, Guid TenantId, string Email, string Type, string Status, DateTimeOffset CreatedAt, DateTimeOffset ExpiresAt, DateTimeOffset? AcceptedAt);
