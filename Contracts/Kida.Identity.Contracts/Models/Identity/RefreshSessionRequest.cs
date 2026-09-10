using Haley.Abstractions;

namespace Kida.Models;
public sealed record RefreshSessionRequest(string RefreshToken, Guid? ClientId = null);
