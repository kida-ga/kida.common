namespace Kida.Models;

public sealed record ReportUsageRequest(
    Guid EventId,
    Guid TenantId,
    Guid ReleaseId,
    string Audience,
    string ProductCode,
    string MeterCode,
    decimal Quantity,
    DateTimeOffset OccurredAt);
