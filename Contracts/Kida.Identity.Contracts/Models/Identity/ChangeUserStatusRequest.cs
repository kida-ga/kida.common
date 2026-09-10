using Haley.Abstractions;

namespace Kida.Models;
public sealed record ChangeUserStatusRequest(Guid UserId, IdentityStatus Status, string ReasonCode);
