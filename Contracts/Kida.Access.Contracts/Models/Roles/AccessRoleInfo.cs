using System.Text;

namespace Kida.Models;
public sealed record AccessRoleInfo(Guid RoleId, Guid TenantId, string Code, string DisplayName, string? Description, string Status, DateTimeOffset CreatedAt, string Resource = "");
