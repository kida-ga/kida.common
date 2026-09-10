namespace Kida.Service.Client;

public sealed class KidaClientOptions
{
    public const string DefaultSectionName = "Kida:Client";

    /// <summary>
    /// Haley.Rest endpoint descriptor, for example
    /// base=https://gateway/;route=security/;suffix=api/;ssl-ignore;.
    /// </summary>
    public string Url { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientIdentifier { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    /// <summary>Audience embedded in user tokens created through this backend.</summary>
    public string UserAudience { get; set; } = string.Empty;
    public int AccessPolicyVersionCheckSeconds { get; set; } = 30;
    public int AccessPolicyMaxStalenessSeconds { get; set; } = 300;
    public int AccessPolicyCacheSlidingSeconds { get; set; } = 900;
    public int AccessPolicyCacheMaxEntries { get; set; } = 10_000;
    public int ClientGrantCacheMaxEntries { get; set; } = 10_000;
    public int SubjectEntitlementCacheMaxEntries { get; set; } = 50_000;
    public int SubjectSetMaxCount { get; set; } = 256;
}
