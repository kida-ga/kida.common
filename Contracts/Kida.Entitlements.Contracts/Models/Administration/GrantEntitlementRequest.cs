using System.Text.Json;

namespace Kida.Models;

public sealed record GrantEntitlementRequest(
    Guid TenantId,
    Guid FeatureId,
    JsonElement Value,
    string Reason,
    DateTimeOffset? ValidFrom,
    DateTimeOffset? ValidUntil);
