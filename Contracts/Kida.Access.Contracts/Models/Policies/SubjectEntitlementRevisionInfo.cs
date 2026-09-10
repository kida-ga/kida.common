namespace Kida.Models;

public sealed record SubjectEntitlementRevisionInfo(
    Guid TenantId,
    string Module,
    string RevisionHash,
    DateTimeOffset? ModifiedAt,
    string Resource = "");
