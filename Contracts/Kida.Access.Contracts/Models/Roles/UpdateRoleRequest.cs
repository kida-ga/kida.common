using System.Text.Json.Serialization;

namespace Kida.Models;

/// <summary>
/// Changes a role's presentation or lifecycle. A null <paramref name="DisplayName"/>,
/// <paramref name="Description"/> or <paramref name="Status"/> leaves that value unchanged;
/// an empty or whitespace description clears it. Status accepts only Active or Inactive;
/// retirement is performed through <see cref="DeleteRoleRequest"/>.
/// </summary>
public sealed record UpdateRoleRequest(
    Guid TenantId,
    Guid RoleId,
    string? DisplayName = null,
    string? Description = null,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus? Status = null,
    Guid? ActorId = null,
    string Resource = "");
