using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record EntitlementFeatureInfo(
    Guid FeatureId,
    Guid ProductId,
    string Code,
    string DisplayName,
    string? Description,
    string ValueType,
    string LicenseMode,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status);
