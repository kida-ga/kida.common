using Haley.Abstractions;

namespace Kida.Models;
public sealed record RevokeSessionRequest(Guid SessionId, string ReasonCode, Guid? ExpectedClientId = null);
