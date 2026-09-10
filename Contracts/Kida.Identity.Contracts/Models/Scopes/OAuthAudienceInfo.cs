namespace Kida.Models;

public sealed record OAuthAudienceInfo(
    Guid AudienceId,
    string Audience,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt);
