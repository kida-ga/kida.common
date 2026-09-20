using System.Text.Json.Serialization;

namespace Kida.Models;

/// <summary>
/// Explicit status bits. Stored lifecycle states must contain one supported bit;
/// the owning operation and database constraint define the valid states for each record.
/// Zero and conflicting combinations are not valid persisted lifecycle states.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<EntitlementStatus>))]
public enum EntitlementStatus : int
{
    Active = 2,
    Retired = 4,
    Suspended = 16,
    Inactive = 32,
    Revoked = 64,
    Expired = 128,
    Deprecated = 256,
    Cancelled = 4096,
    Trial = 2097152,
    PastDue = 4194304,
    Released = 8388608,
}
