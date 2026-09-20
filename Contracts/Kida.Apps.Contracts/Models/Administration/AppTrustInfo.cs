using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record AppTrustInfo(Guid AppId, string AppCode, Guid TrustId, string KeyId, [property: JsonConverter(typeof(JsonNumberEnumConverter<AppStatus>))] AppStatus Status, DateTimeOffset? VerifiedAt);
