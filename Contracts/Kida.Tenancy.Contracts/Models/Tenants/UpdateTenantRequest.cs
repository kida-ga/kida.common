using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record UpdateTenantRequest(
    string DisplayName,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status);
