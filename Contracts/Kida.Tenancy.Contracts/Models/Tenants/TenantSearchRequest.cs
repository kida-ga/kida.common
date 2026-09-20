using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record TenantSearchRequest(
    string? Query = null,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus? Status = null,
    int Page = 1,
    int PageSize = 10);
