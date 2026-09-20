using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record ClientResourceGrantRevision(
    Guid ClientId,
    string Audience,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status,
    ulong Version,
    string RevisionHash,
    DateTimeOffset ModifiedAt);
