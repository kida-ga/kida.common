using System.Text.Json.Serialization;
using System.Text;

namespace Kida.Models;
public sealed record AccessModuleInfo(Guid ModuleId, string Code, string DisplayName, string OwnerFamily, [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus Status, IReadOnlyCollection<AccessModuleVersionInfo> Versions, IReadOnlyCollection<AccessActionInfo> Actions, IReadOnlyCollection<AccessScopeActionInfo> ScopeActions);
