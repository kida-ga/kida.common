namespace Kida.Models;

public sealed record EvaluateEntitlementRequest(
    Guid TenantId,
    Guid ReleaseId,
    string Audience,
    string ProductCode,
    string FeatureCode,
    string? SubjectType = null,
    Guid? SubjectId = null);
