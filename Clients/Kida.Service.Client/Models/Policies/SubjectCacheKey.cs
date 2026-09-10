namespace Kida.Service.Client;

internal readonly record struct SubjectCacheKey(
    Guid TenantId,
    string Resource,
    string Module,
    string SubjectSetKey,
    string ScopePathKey);
