using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record TenantDeploymentInfo(Guid TenantId, Guid DeploymentId, string DeploymentCode, string FamilyCode, string Profile, [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status, DateTimeOffset EffectiveFrom, DateTimeOffset? EffectiveUntil);
