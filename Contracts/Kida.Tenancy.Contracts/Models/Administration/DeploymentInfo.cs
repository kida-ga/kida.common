namespace Kida.Models;

public sealed record DeploymentInfo(Guid DeploymentId, string Code, string Profile, string? Region, string? ResidencyCode, string? BaseUri, string Status, DateTimeOffset CreatedAt, DateTimeOffset ModifiedAt);
