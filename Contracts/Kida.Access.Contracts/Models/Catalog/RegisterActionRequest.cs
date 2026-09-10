using System.Text;

namespace Kida.Models;
/// <summary>Describes an action whose UUID is derived by Kida from module/action codes.</summary>
public sealed record RegisterActionRequest(string Code, string DisplayName, string? Description = null, string RiskLevel = "normal");
