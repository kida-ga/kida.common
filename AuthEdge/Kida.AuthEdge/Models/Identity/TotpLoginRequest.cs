using Haley.Models;

namespace Kida.Models;

public sealed record TotpLoginRequest(string Email, string Code, Guid? MethodId = null,
    MfaKind Kind = MfaKind.Totp, string? DeviceIdentifier = null);
