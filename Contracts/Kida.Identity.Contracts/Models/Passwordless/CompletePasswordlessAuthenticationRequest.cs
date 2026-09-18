namespace Kida.Models;

public sealed record CompletePasswordlessAuthenticationRequest(
    Guid ChallengeId,
    Guid ClientId,
    string Resource,
    string Method,
    string? Code = null,
    string? ActivationToken = null,
    string? ReturnUri = null,
    string? State = null,
    MfaKind? MfaKind = null,
    Guid? MfaMethodId = null,
    string? MfaCode = null,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceIdentifier = null);
