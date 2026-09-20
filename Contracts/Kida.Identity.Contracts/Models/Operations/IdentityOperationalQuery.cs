using System.Text.Json.Serialization;
namespace Kida.Models;
public sealed record IdentityOperationalQuery(IdentityOperationalArea Area, string? Query = null, [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityOperationalStatus>))] IdentityOperationalStatus? Status = null, int Page = 1, int PageSize = 20);
