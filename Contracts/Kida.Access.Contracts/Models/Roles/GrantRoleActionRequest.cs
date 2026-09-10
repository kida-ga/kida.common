using System.Text;

namespace Kida.Models;
public sealed record GrantRoleActionRequest(Guid TenantId, Guid RoleId, string Module, string Action, AccessEffect Effect, string? ConditionPayload = null, Guid? ActorId = null, string Resource = "");
