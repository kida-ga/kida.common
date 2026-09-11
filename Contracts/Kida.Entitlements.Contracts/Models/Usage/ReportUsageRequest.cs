namespace Kida.Models;

public sealed record ReportUsageRequest(
    Guid EventId,
    Guid TenantId,
    string Audience,
    string ProductCode,
    string MeterCode,
    decimal Quantity,
    DateTimeOffset OccurredAt);
