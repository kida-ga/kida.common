namespace Kida.Models;

public sealed record UserMfaPolicyOverride(
    Guid UserId,
    Guid ClientId,
    string Resource,
    string Requirement,
    DateTimeOffset ModifiedAt);
