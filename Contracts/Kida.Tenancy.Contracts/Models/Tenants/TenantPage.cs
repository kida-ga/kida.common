namespace Kida.Models;

public sealed record TenantPage(
    IReadOnlyCollection<TenantSummary> Tenants,
    int Page,
    int PageSize,
    long TotalCount);
