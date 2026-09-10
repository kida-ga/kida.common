namespace Kida.Models;
public sealed record OAuthModuleVersionInfo(Guid VersionId, string Version, IReadOnlyCollection<string> Audiences, DateTimeOffset FirstSeenAt, DateTimeOffset LastSeenAt);
