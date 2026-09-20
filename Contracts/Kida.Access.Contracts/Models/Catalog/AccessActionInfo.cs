using System.Text.Json.Serialization;
using System.Text;

namespace Kida.Models;
public sealed record AccessActionInfo(Guid ActionId, string Code, string DisplayName, string? Description, string RiskLevel, [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus Status);
