using System.Threading.RateLimiting;
using Kida.Constants;
using Kida.Models;
using Kida.Service.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kida.Extensions;

public static class KidaAuthEdgeRegistration
{
    public static IServiceCollection AddKidaAuthEdge(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = KidaAuthEdgeOptions.SectionName,
        string kidaClientSectionName = KidaClientOptions.DefaultSectionName,
        Action<KidaAuthEdgeOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        if (!services.Any(descriptor => descriptor.ServiceType == typeof(IKidaClient)) ||
            !services.Any(descriptor => descriptor.ServiceType == typeof(IKidaAuthEdgeClient)))
        {
            services.AddKidaClient(configuration, kidaClientSectionName);
        }

        var edge = services.AddOptions<KidaAuthEdgeOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(
                static value => IsSafePrefix(value.RoutePrefix) &&
                                IsSafePrefix(value.BrowserRoutePrefix) &&
                                value.MinimumPasswordLength is >= 9 and <= 200,
                "Kida Auth Edge route prefixes must be absolute local paths without wildcards, query strings, or fragments.");
        if (configure is not null) edge.Configure(configure);
        edge.ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            AddPolicy(options, KidaAuthEdgeRateLimits.ClientToken, 5);
            AddPolicy(options, KidaAuthEdgeRateLimits.Authentication, 10);
            AddPolicy(options, KidaAuthEdgeRateLimits.Password, 5);
            AddPolicy(options, KidaAuthEdgeRateLimits.Federation, 10);
        });
        return services;
    }

    private static void AddPolicy(
        RateLimiterOptions options,
        string policyName,
        int requestsPerMinute) =>
        options.AddPolicy(policyName, context =>
            RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = requestsPerMinute,
                    QueueLimit = 0,
                    Window = TimeSpan.FromMinutes(1)
                }));

    private static bool IsSafePrefix(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.StartsWith("/", StringComparison.Ordinal) &&
        value.Length > 1 &&
        !value.Contains("*", StringComparison.Ordinal) &&
        !value.Contains("?", StringComparison.Ordinal) &&
        !value.Contains("#", StringComparison.Ordinal) &&
        !value.Contains("//", StringComparison.Ordinal);
}
