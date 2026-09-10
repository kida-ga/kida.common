namespace Kida.Models;

public sealed record TotpEnrollmentCompletion(
    Guid MethodId,
    IReadOnlyCollection<string> RecoveryCodes,
    string? ReturnUri);
