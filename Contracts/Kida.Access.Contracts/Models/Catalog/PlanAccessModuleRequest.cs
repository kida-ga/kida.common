using System.Text;

namespace Kida.Models;
/// <summary>
/// Pre-authorizes a future module code. Kida derives the planned module UUID.
/// </summary>
public sealed record PlanAccessModuleRequest(string Code, string DisplayName, string OwnerFamily);
