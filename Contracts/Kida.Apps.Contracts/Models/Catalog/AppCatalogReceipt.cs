using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record AppCatalogReceipt(Guid AppId, Guid VersionId, string Code, string Version, string ContentHash, [property: JsonConverter(typeof(JsonNumberEnumConverter<AppStatus>))] AppStatus Status);
