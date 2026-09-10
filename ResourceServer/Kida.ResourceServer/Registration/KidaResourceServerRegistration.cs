using System.IdentityModel.Tokens.Jwt;
using Haley.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kida.ResourceServer;

public static class KidaResourceServerRegistration
{
    public static IServiceCollection AddKidaResourceServer(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = KidaResourceServerOptions.DefaultSectionName,
        string scheme = KidaResourceServerOptions.DefaultScheme,
        Action<KidaResourceServerOptions>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);
        var options = services.AddOptions<KidaResourceServerOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(IsValid, "Kida resource-server validation, Audience, JwksUri, and token-use settings are invalid.");
        if (configure is not null) options.Configure(configure);
        options.ValidateOnStart();

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<KidaJwksTransport>();
        services.TryAddSingleton<KidaJwksCache>();
        services.AddAuthentication(scheme)
            .AddJwtBearerScheme(scheme, ResolveValidationParametersAsync);
        return services;
    }

    private static bool IsValid(KidaResourceServerOptions options) =>
        (!options.ValidateIssuer || !string.IsNullOrWhiteSpace(options.Issuer)) &&
        (!options.ValidateAudience || !string.IsNullOrWhiteSpace(options.Audience)) &&
        options.JwksUri is { IsAbsoluteUri: true } &&
        options.AllowedTokenUses.Length > 0 &&
        options.AllowedTokenUses.All(value => !string.IsNullOrWhiteSpace(value)) &&
        options.JwksRefreshSeconds is >= 30 and <= 86_400 &&
        options.ClockSkewSeconds is >= 0 and <= 300;

    private static async ValueTask<TokenValidationParameters?> ResolveValidationParametersAsync(
        Microsoft.AspNetCore.Http.HttpContext context,
        string token,
        CancellationToken cancellationToken)
    {
        if (token.Length > 16_384) return null;

        string? keyId;
        try
        {
            keyId = new JwtSecurityTokenHandler().ReadJwtToken(token).Header.Kid;
        }
        catch (ArgumentException)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(keyId)) return null;
        var cache = context.RequestServices.GetRequiredService<KidaJwksCache>();
        var keys = await cache.GetKeysAsync(forceRefresh: false, cancellationToken).ConfigureAwait(false);
        if (!keys.Any(key => string.Equals(key.KeyId, keyId, StringComparison.Ordinal)))
        {
            keys = await cache.GetKeysAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        }

        if (!keys.Any(key => string.Equals(key.KeyId, keyId, StringComparison.Ordinal))) return null;
        var options = context.RequestServices.GetRequiredService<IOptions<KidaResourceServerOptions>>().Value;
        return CreateValidationParameters(options, keys);
    }

    internal static TokenValidationParameters CreateValidationParameters(
        KidaResourceServerOptions options,
        IReadOnlyCollection<SecurityKey> signingKeys)
    {
        var allowedUses = new HashSet<string>(options.AllowedTokenUses, StringComparer.Ordinal);
        return KidaTokenValidationParameters.Create(
            options,
            options.Issuer,
            options.Audience,
            signingKeys,
            jwt =>
                !string.IsNullOrWhiteSpace(jwt.Subject) &&
                !string.IsNullOrWhiteSpace(jwt.Id) &&
                jwt.Claims.Any(claim =>
                    string.Equals(claim.Type, "token_use", StringComparison.Ordinal) &&
                    allowedUses.Contains(claim.Value)));
    }
}
