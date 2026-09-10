using System.Text;

namespace Kida.Models;
public sealed record AssignRoleRequest(Guid TenantId, Guid RoleId, string SubjectType, Guid SubjectId, string ScopeType = "tenant", Guid? ScopeId = null, DateTimeOffset? ValidFrom = null, DateTimeOffset? ValidUntil = null, Guid? ActorId = null, uint Flags = AccessAssignmentFlags.AppliesToDescendants, string? Reason = null, string Resource = "");
