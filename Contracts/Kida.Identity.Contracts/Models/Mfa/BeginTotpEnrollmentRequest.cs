using Haley.Abstractions;

namespace Kida.Models;
public sealed record BeginTotpEnrollmentRequest(
    Guid UserId,
    string AccountLabel,
    string? ReturnUri = null,
    Guid? ReplaceMethodId = null,
    Guid? ClientId = null,
    string? Audience = null);
