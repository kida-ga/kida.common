using System.Text.Json.Serialization;

namespace Kida.Models;

/// <summary>
/// Explicit status bits. Stored lifecycle states must contain one supported bit;
/// the owning operation and database constraint define the valid states for each record.
/// Zero and conflicting combinations are not valid persisted lifecycle states.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<AppHealthStatus>))]
public enum AppHealthStatus : int
{
    Unknown = 1,
    Healthy = 2,
    Degraded = 4,
    Unhealthy = 8,
    Offline = 16,
}
