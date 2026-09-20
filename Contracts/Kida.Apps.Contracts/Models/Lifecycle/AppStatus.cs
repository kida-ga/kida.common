using System.Text.Json.Serialization;

namespace Kida.Models;

/// <summary>
/// Explicit status bits. Stored lifecycle states must contain one supported bit;
/// the owning operation and database constraint define the valid states for each record.
/// Zero and conflicting combinations are not valid persisted lifecycle states.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<AppStatus>))]
public enum AppStatus : int
{
    Pending = 1,
    Active = 2,
    Retired = 4,
    Suspended = 16,
    Inactive = 32,
    Revoked = 64,
    Expired = 128,
    Deprecated = 256,
    Disabled = 16777216,
    Uninstalled = 33554432,
}
