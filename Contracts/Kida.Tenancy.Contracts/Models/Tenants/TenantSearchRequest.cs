namespace Kida.Models;

public sealed record TenantSearchRequest(
    string? Query = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 10);
