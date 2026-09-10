using System.Security.Claims;
using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Kida.ResourceServer;

public static class KidaResourceAuthorization
{
    public static bool HasTokenUse(ClaimsPrincipal principal, string tokenUse) =>
        principal.HasClaim(KidaClaimTypes.TokenUse, tokenUse);

    public static bool HasScope(ClaimsPrincipal principal, string requiredScope) =>
        principal.FindAll(KidaClaimTypes.Scope)
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Contains(requiredScope, StringComparer.Ordinal);

    /// <summary>
    /// Adds a machine-client policy. A user assertion can never satisfy this
    /// policy even if a malformed or legacy user JWT contains a scope claim.
    /// </summary>
    public static IServiceCollection AddKidaClientScopePolicy(
        this IServiceCollection services,
        string policyName,
        string requiredScope)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredScope);
        services.AddAuthorization(options => options.AddPolicy(
            policyName,
            policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(KidaClaimTypes.TokenUse, KidaTokenUses.Client)
                .RequireAssertion(context => HasScope(context.User, requiredScope))));
        return services;
    }

    [Obsolete("Use AddKidaClientScopePolicy. Scope claims are authority only on token_use=client tokens.")]
    public static IServiceCollection AddKidaScopePolicy(
        this IServiceCollection services,
        string policyName,
        string requiredScope) =>
        AddKidaClientScopePolicy(services, policyName, requiredScope);
}
