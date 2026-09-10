using Haley.Abstractions;

namespace Kida.Models;
public sealed record IdentityClientPolicy(
    Guid ClientId,
    string Resource,
    EmailVerificationMode EmailVerification,
    int GraceSeconds,
    bool AllowLegacyUserCreate,
    string MfaRequirement,
    DateTimeOffset ModifiedAt,
    IReadOnlyCollection<MfaDomainRule> MfaDomainRules = null!);
