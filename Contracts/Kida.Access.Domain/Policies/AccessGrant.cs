using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
namespace Kida.Utils;
public sealed record AccessGrant(Guid AssignmentId, AccessEffect Effect, string ScopeType, Guid? ScopeId, DateTimeOffset ValidFrom, DateTimeOffset? ValidUntil, string? ConditionPayload, uint Flags = AccessAssignmentFlags.None);
