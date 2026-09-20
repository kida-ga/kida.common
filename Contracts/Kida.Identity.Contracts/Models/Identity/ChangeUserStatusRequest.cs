using System.Text.Json.Serialization;
using Haley.Abstractions;

namespace Kida.Models;
public sealed record ChangeUserStatusRequest(Guid UserId, [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityStatus>))] IdentityStatus Status, string ReasonCode);
