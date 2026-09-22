using Haley.Models;
using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record OAuthAudienceInfo(
    Guid AudienceId,
    string Audience,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt);
