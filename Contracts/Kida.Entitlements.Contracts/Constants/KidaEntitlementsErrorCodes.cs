namespace Kida.Constants;

public static class KidaEntitlementsErrorCodes
{
    public const string InvalidRequest = "entitlements.invalid_request";
    public const string NotFound = "entitlements.not_found";
    public const string Conflict = "entitlements.conflict";
    public const string NotEntitled = "entitlements.not_entitled";
    public const string CapacityExhausted = "entitlements.capacity_exhausted";
    public const string LeaseExpired = "entitlements.lease_expired";
    public const string AudienceForbidden = "entitlements.audience_forbidden";
}
