namespace Kida.Models;

public sealed record AppTrustInfo(Guid AppId, string AppCode, Guid TrustId, string KeyId, string Status, DateTimeOffset? VerifiedAt);
