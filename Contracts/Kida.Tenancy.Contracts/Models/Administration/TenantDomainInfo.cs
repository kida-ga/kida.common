namespace Kida.Models;

public sealed record TenantDomainInfo(Guid DomainId, Guid TenantId, string Name, bool IsPrimary, DateTimeOffset? VerifiedAt, DateTimeOffset CreatedAt);
