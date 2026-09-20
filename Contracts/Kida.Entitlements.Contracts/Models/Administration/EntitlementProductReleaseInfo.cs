using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record EntitlementProductReleaseInfo(
    Guid ReleaseId,
    Guid ProductId,
    string Version,
    string ContentHash,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    DateTimeOffset FirstRegisteredAt,
    DateTimeOffset LastRegisteredAt,
    int FeatureCount,
    int MeterCount);
