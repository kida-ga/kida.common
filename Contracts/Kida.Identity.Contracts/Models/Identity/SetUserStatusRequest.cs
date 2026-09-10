using Haley.Abstractions;

namespace Kida.Models;

public sealed record SetUserStatusRequest(IdentityStatus Status, string ReasonCode);
