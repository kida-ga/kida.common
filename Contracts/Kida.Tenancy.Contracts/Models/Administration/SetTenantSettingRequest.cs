using System.Text.Json;

namespace Kida.Models;

public sealed record SetTenantSettingRequest(Guid TenantId, string Key, JsonElement Value, Guid? ModifiedByUserId);
