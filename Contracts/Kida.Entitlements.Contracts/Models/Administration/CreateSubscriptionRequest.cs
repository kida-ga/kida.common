using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record CreateSubscriptionRequest(
    Guid TenantId,
    Guid ProductId,
    Guid PlanId,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status,
    DateTimeOffset StartsAt,
    DateTimeOffset? TrialEndsAt,
    DateTimeOffset? EndsAt,
    string? BillingReference);
