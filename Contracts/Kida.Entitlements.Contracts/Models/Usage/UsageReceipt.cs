namespace Kida.Models;

public sealed record UsageReceipt(
    Guid EventId,
    decimal PeriodQuantity,
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    bool Duplicate);
