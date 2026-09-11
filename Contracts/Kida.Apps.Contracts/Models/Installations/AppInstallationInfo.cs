namespace Kida.Models;

public sealed record AppInstallationInfo(
    Guid InstallationId,
    Guid TenantId,
    Guid AppId,
    Guid VersionId,
    Guid? OAuthClientId,
    string AppCode,
    string Version,
    string Status,
    DateTimeOffset InstalledAt);
