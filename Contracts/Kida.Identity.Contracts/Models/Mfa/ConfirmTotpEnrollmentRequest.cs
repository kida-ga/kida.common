using Haley.Abstractions;

namespace Kida.Models;
public sealed record ConfirmTotpEnrollmentRequest(Guid UserId, Guid MethodId, string Code);
