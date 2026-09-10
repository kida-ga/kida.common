namespace Kida.Models;

/// <summary>
/// Durable authorization ceiling for one OAuth client and exactly one token audience.
/// </summary>
public sealed record ClientResourceGrantSnapshot(
    Guid ClientId,
    string Audience,
    IReadOnlyCollection<string> AllowedScopes,
    string Status,
    ulong Version,
    string RevisionHash,
    DateTimeOffset ModifiedAt);
