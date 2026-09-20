using System.Text.Json.Serialization;
using System.Text;

namespace Kida.Models;
public sealed record RoleAssignmentInfo(Guid AssignmentId, Guid TenantId, Guid RoleId, string RoleCode, string SubjectType, Guid SubjectId, string ScopeType, Guid? ScopeId, [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus Status, DateTimeOffset ValidFrom, DateTimeOffset? ValidUntil, DateTimeOffset? RevokedAt, uint Flags = AccessAssignmentFlags.None, string Resource = "");
