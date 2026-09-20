using System.Text.Json.Serialization;
using System.Text;

namespace Kida.Models;
public sealed record AccessRoleInfo(Guid RoleId, Guid TenantId, string Code, string DisplayName, string? Description, [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus Status, DateTimeOffset CreatedAt, string Resource = "");
