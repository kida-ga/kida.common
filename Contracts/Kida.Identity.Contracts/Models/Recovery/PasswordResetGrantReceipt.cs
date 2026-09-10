namespace Kida.Models;

public sealed record PasswordResetGrantReceipt(Guid GrantId, DateTimeOffset ExpiresAt);
