using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record TenantInvitationInfo(Guid InvitationId, Guid TenantId, string Email, string Type, [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status, DateTimeOffset CreatedAt, DateTimeOffset ExpiresAt, DateTimeOffset? AcceptedAt);
