using System.Text;

namespace Kida.Models;
public sealed record RoleAssignmentInfo(Guid AssignmentId, Guid TenantId, Guid RoleId, string RoleCode, string SubjectType, Guid SubjectId, string ScopeType, Guid? ScopeId, string Status, DateTimeOffset ValidFrom, DateTimeOffset? ValidUntil, DateTimeOffset? RevokedAt, uint Flags = AccessAssignmentFlags.None, string Resource = "");
