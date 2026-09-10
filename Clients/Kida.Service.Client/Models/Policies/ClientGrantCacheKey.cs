namespace Kida.Service.Client;

internal readonly record struct ClientGrantCacheKey(Guid ClientId, string Audience);
