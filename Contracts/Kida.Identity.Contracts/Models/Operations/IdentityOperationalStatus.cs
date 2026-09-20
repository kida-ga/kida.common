using System.Text.Json.Serialization;

namespace Kida.Models;

[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<IdentityOperationalStatus>))]
public enum IdentityOperationalStatus : int
{
    Pending = 1,
    Active = 2,
    Retired = 4,
    Locked = 8,
    Suspended = 16,
    Inactive = 32,
    Revoked = 64,
    Expired = 128,
    Deprecated = 256,
    Ended = 512,
    Verified = 1024,
    Consumed = 2048,
    Cancelled = 4096,
    Exhausted = 8192,
    Staged = 16384,
    Retiring = 32768,
    Released = 8388608,
    Recorded = 65536,
    Linked = 131072,
    Leased = 262144,
    Processed = 524288,
    Failed = 1048576,
    Succeeded = 2097152,
    Blocked = 4194304,
    PasswordChangeRequired = 16777216,
    MfaRequired = 33554432,
    Unknown = 67108864,
}
