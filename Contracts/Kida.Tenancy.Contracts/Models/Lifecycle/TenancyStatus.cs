using System.Text.Json.Serialization;

namespace Kida.Models;

/// <summary>
/// Explicit status bits. Stored lifecycle states must contain one supported bit;
/// the owning operation and database constraint define the valid states for each record.
/// Zero and conflicting combinations are not valid persisted lifecycle states.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))]
public enum TenancyStatus : int
{
    Pending = 1,
    Active = 2,
    Retired = 4,
    Suspended = 16,
    Inactive = 32,
    Revoked = 64,
    Expired = 128,
    Cancelled = 4096,
    Retiring = 32768,
    Provisioning = 131072,
    Invited = 262144,
    Left = 524288,
    Accepted = 1048576,
}
