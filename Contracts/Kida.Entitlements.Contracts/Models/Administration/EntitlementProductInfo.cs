namespace Kida.Models;

public sealed record EntitlementProductInfo(
    Guid ProductId,
    string Code,
    string DisplayName,
    string? Description,
    string Status,
    IReadOnlyCollection<string> Audiences,
    long Revision);
