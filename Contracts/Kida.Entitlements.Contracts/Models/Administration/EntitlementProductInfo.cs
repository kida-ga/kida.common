using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record EntitlementProductInfo(
    Guid ProductId,
    string Code,
    string DisplayName,
    string? Description,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    string Audience,
    long Revision);
