namespace Kida.Service.Client;

public sealed record KidaScopeCatalogStatusSnapshot(
    bool Enabled,
    string Status,
    DateTimeOffset? LastAttemptUtc,
    DateTimeOffset? LastSuccessUtc,
    string? FailureType);
