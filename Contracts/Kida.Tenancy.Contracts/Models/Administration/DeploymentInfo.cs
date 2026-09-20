using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record DeploymentInfo(Guid DeploymentId, string Code, string Profile, string? Region, string? ResidencyCode, string? BaseUri, [property: JsonConverter(typeof(JsonNumberEnumConverter<TenancyStatus>))] TenancyStatus Status, DateTimeOffset CreatedAt, DateTimeOffset ModifiedAt);
