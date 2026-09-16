namespace Kida.Models;

public sealed record EntitlementSubscriptionInfo(
    Guid SubscriptionId,
    Guid TenantId,
    Guid ProductId,
    Guid PlanId,
    string Status,
    DateTimeOffset StartsAt,
    DateTimeOffset? TrialEndsAt,
    DateTimeOffset? EndsAt,
    string? BillingReference = null);
