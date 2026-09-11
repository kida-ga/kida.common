namespace Kida.Models;

public sealed record AppSummary(
    Guid AppId,
    string Code,
    string DisplayName,
    string? Description,
    string Type,
    string Status,
    string? LatestVersion,
    DateTimeOffset ModifiedAt);
