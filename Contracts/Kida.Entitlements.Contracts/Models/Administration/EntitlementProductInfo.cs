namespace Kida.Models;

public sealed record EntitlementProductInfo(
    Guid ProductId,
    string Code,
    string DisplayName,
    string? Description,
    string Status,
    string Audience,
    long Revision);
