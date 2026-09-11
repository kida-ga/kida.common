namespace Kida.Models;

public sealed record AddTenantDomainRequest(Guid TenantId, string Name, bool IsPrimary = false);
