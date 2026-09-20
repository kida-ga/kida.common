using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record UpdateLicensePoolRequest(
    int Quantity,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    string? DisplayName);
