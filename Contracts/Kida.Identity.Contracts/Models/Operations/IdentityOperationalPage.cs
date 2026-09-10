namespace Kida.Models;
public sealed record IdentityOperationalPage(IReadOnlyCollection<IdentityOperationalRecord> Records, IdentityOperationalArea Area, int Page, int PageSize, long TotalCount);
