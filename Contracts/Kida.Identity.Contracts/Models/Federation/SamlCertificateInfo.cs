namespace Kida.Models;

public sealed record SamlCertificateInfo(
    string Name,
    string Subject,
    string Issuer,
    string SerialNumber,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidTo,
    string Sha256Fingerprint,
    long Size,
    string Status,
    DateTimeOffset ModifiedAt,
    IReadOnlyCollection<string> ReferencingProviders);
