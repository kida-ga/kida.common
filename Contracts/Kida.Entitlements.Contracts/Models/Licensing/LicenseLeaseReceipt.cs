using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record LicenseLeaseReceipt(
    Guid LeaseId,
    Guid PoolId,
    Guid SubjectId,
    DateTimeOffset AcquiredAt,
    DateTimeOffset ExpiresAt,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))] EntitlementStatus Status);
