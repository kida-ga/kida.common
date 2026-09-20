using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record AppVersionInfo(
    Guid VersionId,
    Guid AppId,
    string Version,
    string Channel,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<AppStatus>))] AppStatus Status,
    DateTimeOffset? ReleasedAt,
    string? RequiredProductCode,
    string? RequiredFeatureCode);
