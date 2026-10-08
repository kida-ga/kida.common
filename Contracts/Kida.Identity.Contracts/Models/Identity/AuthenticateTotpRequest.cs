using Haley.Models;

namespace Kida.Models;

/// <summary>Primary authenticator login for one authenticated client and resource.</summary>
public sealed record AuthenticateTotpRequest(string Email, string Code, Guid ClientId, string Resource,
    Guid? MethodId = null, MfaKind Kind = MfaKind.Totp, string? IpAddress = null,
    string? UserAgent = null, string? DeviceIdentifier = null);
