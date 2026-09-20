using System.Text.Json.Serialization;
using System.Text;

namespace Kida.Models;
public sealed record AccessScopeActionInfo(Guid MappingId, string Scope, Guid ActionId, string Action, [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus Status, IReadOnlyCollection<string> RegisteredVersions);
