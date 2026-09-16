using System.Text.Json;

namespace Kida.Models;

public sealed record EntitlementPlanInfo(
    Guid PlanId,
    Guid ProductId,
    string Code,
    string DisplayName,
    string? BillingPeriod,
    string Status,
    IReadOnlyDictionary<string, JsonElement> Features);
