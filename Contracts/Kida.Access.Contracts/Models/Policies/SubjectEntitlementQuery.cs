namespace Kida.Models;

/// <summary>
/// A product-authenticated subject set and ordered resource path. Group identifiers
/// are opaque product-owned UUIDs and must be supplied by the trusted backend.
/// </summary>
public sealed record SubjectEntitlementQuery(
    string Resource,
    string Module,
    IReadOnlyCollection<AccessSubjectRef> Subjects,
    IReadOnlyCollection<AccessScopeRef> ScopePath);
