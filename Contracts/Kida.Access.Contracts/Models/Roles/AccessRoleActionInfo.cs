using System.Text;

namespace Kida.Models;
public sealed record AccessRoleActionInfo(Guid RoleId, string Module, string Action, AccessEffect Effect, string? ConditionPayload, DateTimeOffset GrantedAt);
