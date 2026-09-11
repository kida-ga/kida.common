namespace Kida.Models;

public sealed record TenantMembershipInfo(Guid MembershipId, Guid TenantId, Guid UserId, string Type, string Status, DateTimeOffset JoinedAt, DateTimeOffset ModifiedAt);
