using Microsoft.IdentityModel.Tokens;

namespace Kida.ResourceServer;

internal sealed record CachedKeys(IReadOnlyCollection<SecurityKey> Keys, DateTimeOffset RefreshAt);
