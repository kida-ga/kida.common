namespace Kida.Models;

public sealed record IdentityOperationalExportProbe(
    IReadOnlyCollection<IdentityOperationalRecord> Records,
    bool LimitExceeded);
