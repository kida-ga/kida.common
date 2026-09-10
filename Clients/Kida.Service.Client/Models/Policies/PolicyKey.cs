namespace Kida.Service.Client;

internal readonly record struct PolicyKey(Guid TenantId, string Resource, string Module);
