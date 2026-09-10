namespace Kida.Models;

public sealed record PasswordResetInitiationResult(
    bool Accepted,
    PasswordResetDeliveryReceipt? Delivery = null);
