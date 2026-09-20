using System.Text.Json.Serialization;
using System.Text.Json;

namespace Kida.Models;

public sealed record ReportAppHealthRequest(
    Guid TenantId,
    string HostAudience,
    Guid InstallationId,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<AppHealthStatus>))] AppHealthStatus Status,
    string? Version,
    int? LatencyMilliseconds,
    JsonElement? Details,
    DateTimeOffset CheckedAt);
