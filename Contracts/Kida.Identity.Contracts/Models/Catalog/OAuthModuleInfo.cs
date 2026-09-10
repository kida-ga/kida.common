namespace Kida.Models;
public sealed record OAuthModuleInfo(Guid ModuleId, string Code, string DisplayName, string? Description, string OwnerFamily, string Status, IReadOnlyCollection<OAuthModuleVersionInfo> Versions, IReadOnlyCollection<string> Audiences, IReadOnlyCollection<OAuthScopeInfo> Scopes, DateTimeOffset CreatedAt, DateTimeOffset ModifiedAt, DateTimeOffset LastRegisteredAt);
