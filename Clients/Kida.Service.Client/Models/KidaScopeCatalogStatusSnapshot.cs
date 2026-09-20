using System.Text.Json.Serialization;

namespace Kida.Service.Client;

public sealed record KidaScopeCatalogStatusSnapshot(
    bool Enabled,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<KidaCatalogRegistrationStatus>))] KidaCatalogRegistrationStatus Status,
    DateTimeOffset? LastAttemptUtc,
    DateTimeOffset? LastSuccessUtc,
    string? FailureType);
