namespace Kida.Models;

public sealed record PasswordlessAuthenticationInitiationResult(
    bool Accepted,
    PasswordlessAuthenticationDeliveryReceipt? Delivery = null);
