namespace Kida.Models;

public sealed record CreateTenantInvitationRequest(Guid TenantId, string Email, string Type, Guid InvitedByUserId, DateTimeOffset ExpiresAt);
