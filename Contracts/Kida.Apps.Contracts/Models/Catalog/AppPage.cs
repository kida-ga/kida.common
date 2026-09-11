namespace Kida.Models;

public sealed record AppPage(IReadOnlyCollection<AppSummary> Items, int Page, int PageSize, long Total);
