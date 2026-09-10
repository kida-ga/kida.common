namespace Kida.Models;

public sealed record RoleActionChange(
    string Module,
    string Action,
    AccessEffect? Effect,
    string? ConditionPayload = null);
