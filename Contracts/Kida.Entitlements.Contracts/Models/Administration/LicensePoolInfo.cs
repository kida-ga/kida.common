using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record LicensePoolInfo(
    Guid PoolId,
    Guid TenantId,
    Guid SubscriptionId,
    Guid? FeatureId,
    string Type,
    int Quantity,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    int Assigned,
    int Leased,
    string? DisplayName = null);
