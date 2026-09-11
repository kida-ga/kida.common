namespace Kida.Models;

public sealed record EntitlementPlanInfo(
    Guid PlanId,
    Guid ProductId,
    string Code,
    string DisplayName,
    string? BillingPeriod,
    string Status);
