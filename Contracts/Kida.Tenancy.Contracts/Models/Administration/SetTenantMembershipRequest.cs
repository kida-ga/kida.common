using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record SetTenantMembershipRequest(Guid TenantId, Guid UserId, string Type, [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status);
