using Haley.Models;
using System.Text.Json.Serialization;
namespace Kida.Models;

/// <summary>
/// Durable authorization ceiling for one OAuth client and exactly one token audience.
/// </summary>
public sealed record ClientResourceGrantSnapshot(
    Guid ClientId,
    string Audience,
    IReadOnlyCollection<string> AllowedScopes,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status,
    ulong Version,
    string RevisionHash,
    DateTimeOffset ModifiedAt);
