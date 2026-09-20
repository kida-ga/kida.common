using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record AppSummary(
    Guid AppId,
    string Code,
    string DisplayName,
    string? Description,
    string Type,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<AppStatus>))] AppStatus Status,
    string? LatestVersion,
    DateTimeOffset ModifiedAt);
