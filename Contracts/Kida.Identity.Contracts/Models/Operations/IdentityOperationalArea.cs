namespace Kida.Models;
/// <summary>
/// Read-only operational areas exposed to trusted Kida administration hosts.
/// Sensitive verifiers, encrypted payloads, assertions, claims, and secrets are
/// deliberately absent from these contracts.
/// </summary>
public enum IdentityOperationalArea
{
    Contacts,
    Credentials,
    Origins,
    Federation,
    Mfa,
    VerificationChallenges,
    VerificationGrants,
    Saml,
    Sessions,
    RefreshFamilies,
    LoginAttempts,
    AccountLocks,
    ClientSecrets,
    TokenRevocations,
    Outbox
}
