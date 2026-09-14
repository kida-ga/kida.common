using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Haley.Utils;

namespace Kida.Service.Client;

public static class KidaClientRegistration
{
    public static IServiceCollection AddKidaClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = KidaClientOptions.DefaultSectionName,
        Action<KidaClientOptions>? configure = null)
    {
        var options = services.AddOptions<KidaClientOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(static value =>
                    IsValidEndpoint(value.Url) &&
                    value.ClientId != Guid.Empty &&
                    !string.IsNullOrWhiteSpace(value.ClientIdentifier) &&
                    !string.IsNullOrWhiteSpace(value.ClientSecret) &&
                    KidaResourceAudience.TryNormalize(value.UserAudience, out _) &&
                    value.AccessPolicyVersionCheckSeconds is >= 5 and <= 3600 &&
                    value.AccessPolicyMaxStalenessSeconds is >= 30 and <= 86_400 &&
                    value.AccessPolicyCacheSlidingSeconds is >= 60 and <= 86_400 &&
                    value.AccessPolicyCacheMaxEntries is >= 100 and <= 100_000 &&
                    value.ClientGrantCacheMaxEntries is >= 100 and <= 100_000 &&
                    value.SubjectEntitlementCacheMaxEntries is >= 100 and <= 500_000 &&
                    value.SubjectSetMaxCount is >= 1 and <= 1024,
                "Kida client Url, ClientId, ClientIdentifier, ClientSecret, and UserAudience are required.");
        if (configure is not null) options.Configure(configure);
        options.ValidateOnStart();
        services.AddOptions<KidaScopeCatalogOptions>()
            .Bind(configuration.GetSection(KidaScopeCatalogConfiguration.SectionName));

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<KidaClientTransport>();
        services.TryAddSingleton<IKidaClientTokenProvider, KidaClientTokenProvider>();
        services.TryAddSingleton<IKidaAccessPolicyCache, KidaAccessPolicyCache>();
        services.TryAddSingleton<KidaScopeCatalogStatus>();
        services.TryAddSingleton<IKidaScopeCatalogStatus>(static provider =>
            provider.GetRequiredService<KidaScopeCatalogStatus>());
        services.TryAddTransient<IKidaClient, KidaClient>();
        services.TryAddTransient<IKidaAuthEdgeClient, KidaClient>();
        var scopeCatalogSection = configuration.GetSection(KidaScopeCatalogConfiguration.SectionName);
        if (scopeCatalogSection.GetValue(nameof(KidaScopeCatalogOptions.RegisterOnStartup), true))
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IHostedService, KidaClientScopeCatalogHostedService>());
        }
        return services;
    }

    public static IServiceCollection AddKidaModuleCatalogProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IKidaModuleCatalogProvider
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IKidaModuleCatalogProvider, TProvider>());
        return services;
    }

    public static IServiceCollection AddKidaIntegrationClientManagement(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = KidaIntegrationManagementOptions.DefaultSectionName,
        Action<KidaIntegrationManagementOptions>? configure = null)
    {
        if (!services.Any(descriptor => descriptor.ServiceType == typeof(KidaClientTransport)))
        {
            throw new InvalidOperationException(
                "Call AddKidaClient before AddKidaIntegrationClientManagement so both clients reuse the Haley transport.");
        }

        var options = services.AddOptions<KidaIntegrationManagementOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(static value =>
                    value.ClientId != Guid.Empty &&
                    !string.IsNullOrWhiteSpace(value.ClientIdentifier) &&
                    !string.IsNullOrWhiteSpace(value.ClientSecret),
                "Kida integration-management ClientId, ClientIdentifier, and ClientSecret are required.");
        if (configure is not null) options.Configure(configure);
        options.ValidateOnStart();
        services.TryAddSingleton<IKidaIntegrationClientManager, KidaIntegrationClientManager>();
        return services;
    }

    private static bool IsValidEndpoint(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        try
        {
            return Uri.TryCreate(
                value.ToDictionarySplit().GenerateBaseURLAddress(),
                UriKind.Absolute,
                out _);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
