namespace Kida.Models;

public sealed record EntitlementSubscriptionPage(
    IReadOnlyCollection<EntitlementSubscriptionInfo> Subscriptions,
    int Page,
    int PageSize,
    bool HasNext);
