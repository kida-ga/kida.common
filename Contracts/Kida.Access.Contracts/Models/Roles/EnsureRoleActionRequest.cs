using System.Text;

namespace Kida.Models;
/// <summary>
/// Startup-only bootstrap request. It creates a missing role/action relationship
/// and never changes an existing relationship's effect, condition, or lifecycle.
/// </summary>
public sealed record EnsureRoleActionRequest(Guid TenantId, Guid RoleId, string Module, string Action, AccessEffect Effect, string? ConditionPayload = null, Guid? ActorId = null, string Resource = "");
