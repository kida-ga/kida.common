using System.Text.Json.Serialization;
using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserSessionInfo(Guid SessionId, Guid UserId, Guid? ClientId, string? Resource, [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status, DateTimeOffset AuthenticatedAt, DateTimeOffset LastSeenAt, DateTimeOffset ExpiresAt, DateTimeOffset? EndedAt);
