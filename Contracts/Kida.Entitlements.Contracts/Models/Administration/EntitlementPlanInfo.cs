using System.Text.Json.Serialization;
using System.Text.Json;

namespace Kida.Models;

public sealed record EntitlementPlanInfo(
    Guid PlanId,
    Guid ProductId,
    string Code,
    string DisplayName,
    string? BillingPeriod,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    IReadOnlyDictionary<string, JsonElement> Features);
