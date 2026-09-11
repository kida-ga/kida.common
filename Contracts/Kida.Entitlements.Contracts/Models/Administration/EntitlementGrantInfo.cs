using System.Text.Json;

namespace Kida.Models;

public sealed record EntitlementGrantInfo(
    Guid GrantId,
    Guid TenantId,
    Guid FeatureId,
    JsonElement Value,
    string Reason,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidUntil,
    DateTimeOffset? RevokedAt);
