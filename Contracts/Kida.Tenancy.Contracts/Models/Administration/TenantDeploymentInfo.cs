namespace Kida.Models;

public sealed record TenantDeploymentInfo(Guid TenantId, Guid DeploymentId, string DeploymentCode, string FamilyCode, string Profile, string Status, DateTimeOffset EffectiveFrom, DateTimeOffset? EffectiveUntil);
