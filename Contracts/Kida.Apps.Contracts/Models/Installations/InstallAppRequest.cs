using System.Text.Json;

namespace Kida.Models;

public sealed record InstallAppRequest(
    Guid TenantId,
    string HostAudience,
    Guid AppId,
    Guid VersionId,
    Guid? OAuthClientId,
    JsonElement? Configuration,
    IReadOnlyCollection<AppInstallationGrant> Grants);
