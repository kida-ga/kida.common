using System.Text.Json;

namespace Kida.Models;

public sealed record TenantSettingInfo(Guid TenantId, string Key, JsonElement Value, int Version, DateTimeOffset ModifiedAt, Guid? ModifiedByUserId);
