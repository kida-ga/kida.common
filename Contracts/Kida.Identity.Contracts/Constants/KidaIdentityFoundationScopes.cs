namespace Kida.Constants;

/// <summary>Required machine-client scopes for the shared Identity surface hosted by Kida.</summary>
public static class KidaIdentityFoundationScopes
{
    public const string AccountsEnsure = "identity.accounts.ensure";
    public const string SessionsIssue = "identity.sessions.issue";
    public const string SessionsValidate = "identity.sessions.validate";
    public const string MfaManage = "identity.mfa.manage";
    public const string MfaVerify = "identity.mfa.verify";
    public const string ResourceHeader = "X-Kida-Identity-Resource";
    public static string ForOperation(string operation) => operation switch
    {
        "BeginVerificationPasswordlessLogin" or "CompleteVerificationPasswordlessLogin" => KidaIdentityScopes.Authenticate,
        "BeginVerificationPasswordReset" or "CompleteVerificationPasswordReset" => KidaIdentityScopes.PasswordResetRequest,
        "BeginVerificationOnboarding" or "CompleteVerificationOnboarding" => KidaIdentityScopes.UsersInvite,
        "BeginVerificationEmailVerification" or "CompleteVerificationEmailVerification" => KidaIdentityScopes.UsersManage,
        "GetEmailVerification" => KidaIdentityScopes.UsersResolve,
        "BeginPasswordReset" or "VerifyPasswordResetCode" or "CompletePasswordReset" => KidaIdentityScopes.PasswordResetRequest,
        "GetAccount" or "FindAccount" or "GetProfile" => KidaIdentityScopes.UsersResolve,
        "EnsureAccount" => AccountsEnsure,
        "AuthenticatePassword" => KidaIdentityScopes.Authenticate,
        "CreateApplicationSession" => SessionsIssue,
        "ValidateSession" => SessionsValidate,
        "RevokeSession" => KidaIdentityScopes.SessionsRevoke,
        "ListMfaMethods" or "BeginTotpEnrollment" or "InspectTotpEnrollment" or "ConfirmTotpEnrollment"
            or "RetireMfaMethod" or "ReplaceRecoveryCodes" => MfaManage,
        "VerifyMfa" => MfaVerify,
        "ListAccounts" or "UpdateProfile" or "SetAccountStatus" or "SetPassword" or "ChangePassword"
            or "ListLoginAttempts" or "ReleaseAccountLock" => KidaIdentityScopes.UsersManage,
        _ => throw new ArgumentOutOfRangeException(nameof(operation), "Unknown Identity operation.")
    };
}
