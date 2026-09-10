namespace Kida.Models;
public sealed record IdentityOperationalQuery(IdentityOperationalArea Area, string? Query = null, string? Status = null, int Page = 1, int PageSize = 20);
