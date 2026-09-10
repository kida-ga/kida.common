namespace Kida.Models;

public sealed record ClientResourceGrantRevision(
    Guid ClientId,
    string Audience,
    string Status,
    ulong Version,
    string RevisionHash,
    DateTimeOffset ModifiedAt);
