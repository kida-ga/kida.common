namespace Kida.Models;

/// <summary>Only assignments applicable to one normalized subject set and path.</summary>
public sealed record SubjectEntitlementSnapshot(
    Guid TenantId,
    string Module,
    string RevisionHash,
    DateTimeOffset GeneratedAt,
    IReadOnlyCollection<AccessSubjectRef> Subjects,
    IReadOnlyCollection<AccessScopeRef> ScopePath,
    IReadOnlyCollection<RoleAssignmentInfo> Assignments,
    string Resource = "");
