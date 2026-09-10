using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Kida.ResourceServer;

public static class KidaTokenValidationParameters
{
    public static TokenValidationParameters Create(
        KidaTokenValidationOptions options,
        string? issuer,
        string? audience,
        IReadOnlyCollection<SecurityKey> signingKeys,
        Func<JwtSecurityToken, bool>? validateToken = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(signingKeys);
        if (signingKeys.Count == 0)
        {
            throw new ArgumentException("At least one trusted Kida signing key is required.", nameof(signingKeys));
        }

        var clockSkew = TimeSpan.FromSeconds(options.ClockSkewSeconds);
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            TryAllIssuerSigningKeys = false,
            RequireSignedTokens = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            IssuerSigningKeyValidator = (resolvedKey, securityToken, _) =>
                securityToken is JwtSecurityToken jwt &&
                !string.IsNullOrWhiteSpace(jwt.Header.Kid) &&
                string.Equals(resolvedKey.KeyId, jwt.Header.Kid, StringComparison.Ordinal) &&
                signingKeys.Any(key => string.Equals(key.KeyId, resolvedKey.KeyId, StringComparison.Ordinal)) &&
                (validateToken?.Invoke(jwt) ?? true),
            ValidateIssuer = options.ValidateIssuer,
            ValidIssuer = options.ValidateIssuer ? issuer : null,
            ValidateAudience = options.ValidateAudience,
            RequireAudience = options.ValidateAudience,
            ValidAudience = options.ValidateAudience ? audience : null,
            AudienceValidator = options.ValidateAudience
                ? (audiences, _, _) =>
                    audiences.Count() == 1 &&
                    audiences.Contains(audience, StringComparer.Ordinal)
                : null,
            ValidateLifetime = options.ValidateLifetime,
            RequireExpirationTime = options.ValidateLifetime,
            ClockSkew = clockSkew,
            LifetimeValidator = options.ValidateLifetime
                ? (notBefore, expires, securityToken, _) =>
                {
                    var now = DateTime.UtcNow;
                    return expires.HasValue && expires.Value >= now.Subtract(clockSkew) &&
                           (!notBefore.HasValue || notBefore.Value <= now.Add(clockSkew)) &&
                           securityToken is JwtSecurityToken jwt &&
                           jwt.Payload.IssuedAt != DateTime.MinValue &&
                           jwt.Payload.IssuedAt <= now.Add(clockSkew);
                }
                : null,
            ValidTypes = ["JWT"],
            NameClaimType = "name",
            RoleClaimType = ClaimTypes.Role
        };
    }
}
