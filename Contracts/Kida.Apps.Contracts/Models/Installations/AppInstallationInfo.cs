using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record AppInstallationInfo(
    Guid InstallationId,
    Guid TenantId,
    Guid AppId,
    Guid VersionId,
    Guid? OAuthClientId,
    string AppCode,
    string Version,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<AppStatus>))] AppStatus Status,
    DateTimeOffset InstalledAt);
