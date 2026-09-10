using System.Text;

namespace Kida.Models;
public sealed record AccessActionInfo(Guid ActionId, string Code, string DisplayName, string? Description, string RiskLevel, string Status);
