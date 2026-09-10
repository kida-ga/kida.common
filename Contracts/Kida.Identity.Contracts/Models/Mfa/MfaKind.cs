using Haley.Abstractions;

namespace Kida.Models;
public enum MfaKind
{
    EmailOtp,
    Saml,
    Totp,
    RecoveryCode
}
