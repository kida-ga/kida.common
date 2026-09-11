using System.Text.Json;

namespace Kida.Models;

public sealed record CreateEntitlementPlanRequest(
    Guid ProductId,
    string Code,
    string DisplayName,
    string? BillingPeriod,
    IReadOnlyDictionary<string, JsonElement> Features);
