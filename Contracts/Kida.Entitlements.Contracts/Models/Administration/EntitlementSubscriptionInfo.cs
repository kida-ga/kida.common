using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record EntitlementSubscriptionInfo(
    Guid SubscriptionId,
    Guid TenantId,
    Guid ProductId,
    Guid PlanId,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    DateTimeOffset StartsAt,
    DateTimeOffset? TrialEndsAt,
    DateTimeOffset? EndsAt,
    string? BillingReference = null,
    string? PlanCode = null,
    string? PlanDisplayName = null,
    DateTimeOffset? CreatedAt = null,
    DateTimeOffset? ModifiedAt = null);
