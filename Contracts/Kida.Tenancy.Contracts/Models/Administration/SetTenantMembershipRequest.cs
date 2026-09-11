namespace Kida.Models;

public sealed record SetTenantMembershipRequest(Guid TenantId, Guid UserId, string Type, string Status);
