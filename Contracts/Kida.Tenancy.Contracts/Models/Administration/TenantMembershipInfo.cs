using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record TenantMembershipInfo(Guid MembershipId, Guid TenantId, Guid UserId, string Type, [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status, DateTimeOffset JoinedAt, DateTimeOffset ModifiedAt);
