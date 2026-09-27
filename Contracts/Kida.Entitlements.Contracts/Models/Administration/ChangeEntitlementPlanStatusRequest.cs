namespace Kida.Models;

/// <summary>
/// Retires a plan, or brings a retired one back. A plan only ever moves between Active and Inactive:
/// deleting one is a separate operation and is refused once any subscription has used it.
/// <see cref="ProductId"/> names the product the plan belongs to, so the caller's audience can be
/// checked the same way creating a plan checks it.
/// </summary>
public sealed record ChangeEntitlementPlanStatusRequest(
    Guid ProductId,
    EntitlementStatus Status);
