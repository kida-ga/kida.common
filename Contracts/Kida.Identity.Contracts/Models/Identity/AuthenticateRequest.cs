using Haley.Models;
using Haley.Abstractions;

namespace Kida.Models;
public sealed record AuthenticateRequest(string Username, string Password, Guid? ClientId = null, string? IpAddress = null, string? UserAgent = null, string? DeviceIdentifier = null, string? Resource = null, Guid? MfaMethodId = null, MfaKind? MfaKind = null, string? MfaCode = null);
