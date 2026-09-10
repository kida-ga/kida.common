namespace Kida.Models;
/// <summary>A fine-grained operation evaluated for a user or service subject.</summary>
public sealed record KidaActionDefinition(string Code, string DisplayName, string? Description = null, string RiskLevel = "normal");
