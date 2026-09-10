using Haley.Abstractions;

namespace Kida.Models;
public sealed record UpsertIdentityClientPolicyRequest(
    EmailVerificationMode EmailVerification = EmailVerificationMode.Immediate,
    int GraceSeconds = 0,
    bool AllowLegacyUserCreate = false,
    string MfaRequirement = "optional",
    IReadOnlyCollection<MfaDomainRule>? MfaDomainRules = null);
