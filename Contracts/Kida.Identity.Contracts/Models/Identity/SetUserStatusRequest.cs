using Haley.Models;
using System.Text.Json.Serialization;
using Haley.Abstractions;

namespace Kida.Models;

public sealed record SetUserStatusRequest([property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityStatus>))] IdentityStatus Status, string ReasonCode);
