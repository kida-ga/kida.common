namespace Kida.Models;

public sealed record CreateSubscriptionRequest(
    Guid TenantId,
    Guid ProductId,
    Guid PlanId,
    string Status,
    DateTimeOffset StartsAt,
    DateTimeOffset? TrialEndsAt,
    DateTimeOffset? EndsAt,
    string? BillingReference);
