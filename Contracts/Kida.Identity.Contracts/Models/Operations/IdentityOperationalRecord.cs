using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record IdentityOperationalRecord(
    string RecordKey,
    IdentityOperationalArea Area,
    string Type,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityOperationalStatus>))] IdentityOperationalStatus Status,
    Guid? RecordId,
    Guid? UserId,
    string? UserDisplayName,
    string? UserEmail,
    Guid? ClientId,
    string? ClientIdentifier,
    string? ClientDisplayName,
    Guid? TenantId,
    string? Provider,
    string? Audience,
    string? Detail,
    int? Attempts,
    long? Generation,
    DateTimeOffset OccurredAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? CompletedAt);
