using System.Text;

namespace Kida.Models;
public sealed record AccessPolicySnapshot(Guid TenantId, string Module, ulong Version, DateTimeOffset GeneratedAt, IReadOnlyCollection<AccessRoleInfo> Roles, IReadOnlyCollection<AccessRoleActionInfo> RoleActions, IReadOnlyCollection<RoleAssignmentInfo> Assignments, string Resource = "");
