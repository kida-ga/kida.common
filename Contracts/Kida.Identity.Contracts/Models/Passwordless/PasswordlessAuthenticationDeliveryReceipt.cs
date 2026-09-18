namespace Kida.Models;

public sealed record PasswordlessAuthenticationDeliveryReceipt(
    Guid ChallengeId,
    string Method,
    string Destination,
    DateTimeOffset ExpiresAt,
    DateTimeOffset ResendAllowedAt,
    string? OneTimeCode = null,
    string? ActivationToken = null);
