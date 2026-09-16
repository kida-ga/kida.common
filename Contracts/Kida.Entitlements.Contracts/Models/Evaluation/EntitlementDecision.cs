using System.Text.Json;

namespace Kida.Models;

public sealed record EntitlementDecision(
    bool Entitled,
    JsonElement? Value,
    string Reason,
    Guid? ReleaseId,
    long ProductRevision,
    long TenantRevision,
    string RevisionHash,
    DateTimeOffset? ValidUntil);
