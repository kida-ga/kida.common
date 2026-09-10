using System.Text;

namespace Kida.Models;
public sealed record CreateRoleRequest(Guid TenantId, string Code, string DisplayName, string? Description, Guid? ActorId = null, string Resource = "");
