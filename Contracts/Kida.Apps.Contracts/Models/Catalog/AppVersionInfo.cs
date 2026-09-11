namespace Kida.Models;

public sealed record AppVersionInfo(
    Guid VersionId,
    Guid AppId,
    string Version,
    string Channel,
    string Status,
    DateTimeOffset? ReleasedAt,
    string? RequiredProductCode,
    string? RequiredFeatureCode);
