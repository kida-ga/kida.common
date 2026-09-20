using System.Text.Json.Serialization;

namespace Kida.Models;

/// <summary>
/// Explicit status bits. Stored lifecycle states must contain one supported bit;
/// the owning operation and database constraint define the valid states for each record.
/// Zero and conflicting combinations are not valid persisted lifecycle states.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))]
public enum AccessStatus : int
{
    Active = 2,
    Retired = 4,
    Inactive = 32,
    Revoked = 64,
    Expired = 128,
    Ended = 512,
    Planned = 65536,
}
