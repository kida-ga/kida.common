using Haley.Abstractions;

namespace Kida.Models;
public sealed record TotpEnrollmentReceipt(
    Guid MethodId,
    string Ticket,
    string BrowserPath,
    DateTimeOffset ExpiresAt);
