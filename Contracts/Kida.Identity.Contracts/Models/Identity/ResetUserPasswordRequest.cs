using Haley.Abstractions;

namespace Kida.Models;
public sealed record ResetUserPasswordRequest(Guid UserId, string NewPassword, bool RequirePasswordChange, string ReasonCode, string ActorReference);
