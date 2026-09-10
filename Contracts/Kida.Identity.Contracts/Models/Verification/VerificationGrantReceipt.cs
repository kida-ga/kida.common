using Haley.Abstractions;

namespace Kida.Models;
public sealed record VerificationGrantReceipt(Guid GrantId, DateTimeOffset ExpiresAt);
