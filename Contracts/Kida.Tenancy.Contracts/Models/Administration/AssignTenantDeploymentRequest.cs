namespace Kida.Models;

public sealed record AssignTenantDeploymentRequest(Guid TenantId, Guid DeploymentId, string FamilyCode, DateTimeOffset? EffectiveFrom = null, DateTimeOffset? EffectiveUntil = null);
