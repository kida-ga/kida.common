using System.Text.Json;

namespace Kida.Models;

public sealed record ReportAppHealthRequest(
    Guid TenantId,
    string HostAudience,
    Guid InstallationId,
    string Status,
    string? Version,
    int? LatencyMilliseconds,
    JsonElement? Details,
    DateTimeOffset CheckedAt);
