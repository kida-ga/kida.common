namespace Kida.Models;

public sealed record TenantSummary(
    Guid TenantId,
    string Code,
    string DisplayName,
    string Type,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt);
