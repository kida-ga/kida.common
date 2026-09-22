using Haley.Models;
using System.Text.Json.Serialization;
namespace Kida.Models;
public sealed record KidaScopeParameterOption(string Value, string DisplayName, [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status = IdentityRecordStatus.Active);
