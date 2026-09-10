using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record ClientTokenResult(bool Succeeded, string? AccessToken = null, DateTimeOffset? ExpiresAt = null, string TokenType = "Bearer", string? Scope = null, string? ErrorCode = null, string? Resource = null)
{
    public static ClientTokenResult Failure(string errorCode) => new(false, ErrorCode: errorCode);
}
