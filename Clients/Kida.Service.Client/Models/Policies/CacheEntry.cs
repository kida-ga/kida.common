using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
namespace Kida.Service.Client;

internal abstract class CacheEntry
{
    public SemaphoreSlim RefreshLock { get; } = new(1, 1);
    public string? RevisionHash;
    public DateTimeOffset LoadedAt;
    public DateTimeOffset LastAccessedAt;
    public long NextVersionCheckUtcTicks;
}

internal sealed class PolicyDefinitionCacheEntry : CacheEntry
{
    public AccessPolicyDefinitionSnapshot? Snapshot;
}

internal sealed class SubjectEntitlementCacheEntry : CacheEntry
{
    public SubjectEntitlementSnapshot? Snapshot;
}

internal sealed class ClientGrantCacheEntry : CacheEntry
{
    public ClientResourceGrantSnapshot? Snapshot;
}
