using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record TenantSummary(
    Guid TenantId,
    string Code,
    string DisplayName,
    string Type,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt);
