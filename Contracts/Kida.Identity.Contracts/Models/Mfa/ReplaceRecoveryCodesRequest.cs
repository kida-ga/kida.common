using Haley.Abstractions;

namespace Kida.Models;
public sealed record ReplaceRecoveryCodesRequest(Guid UserId, int Count = 10);
