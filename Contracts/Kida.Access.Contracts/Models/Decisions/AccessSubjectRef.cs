namespace Kida.Models;

/// <summary>
/// One opaque authorization subject. Kida understands the type and identifier
/// only as access keys; the product remains authoritative for group membership.
/// </summary>
public sealed record AccessSubjectRef(string Type, Guid Id);
