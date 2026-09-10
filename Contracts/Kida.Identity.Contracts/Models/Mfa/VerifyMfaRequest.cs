using Haley.Abstractions;

namespace Kida.Models;
public sealed record VerifyMfaRequest(Guid UserId, Guid? MethodId, MfaKind Kind, string Code);
