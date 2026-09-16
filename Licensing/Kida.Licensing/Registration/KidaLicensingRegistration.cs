using Haley.Models;
using Haley.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Kida.Licensing;

public static class KidaLicensingRegistration
{
    public const string ConfigurationSection = "Kida:Licensing";

    /// <summary>
    /// Registers the product license with Kida's common defaults and optional <c>Kida:Licensing</c> overrides.
    /// Product hosts normally use this overload and provide only their product, feature and limit catalog.
    /// </summary>
    public static IServiceCollection AddKidaLicensing(
        this IServiceCollection services,
        IConfiguration configuration,
        KidaLicenseProduct product,
        AppFlags? flags = null,
        Action<KidaLicenseOptions>? configure = null) =>
        AddKidaLicensing(services, configuration, product, ConfigurationSection, flags, configure);

    /// <summary>
    /// Registers the product license from an explicitly selected configuration section. This overload is retained
    /// for hosts that require a compatibility or product-specific section.
    /// </summary>
    /// <param name="flags">
    /// The host's application flags. When the glass has been broken in code (<see cref="AppFlags.BreakGlass"/>),
    /// no license file is read and every compiled feature is available, so a missing or expired grant cannot stop
    /// the host. The flag cannot come from configuration.
    /// </param>
    public static IServiceCollection AddKidaLicensing(
        this IServiceCollection services,
        IConfiguration configuration,
        KidaLicenseProduct product,
        string sectionName,
        AppFlags? flags = null,
        Action<KidaLicenseOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.TryAddSingleton(product);
        services.TryAddSingleton(TimeProvider.System);
        var options = services.AddOptions<KidaLicenseOptions>()
            .Bind(configuration.GetSection(sectionName))
            .PostConfigure(value =>
            {
                value.TrialDays = LicensePolicyLimits.CapTrialDays(value.TrialDays);
                if (string.IsNullOrWhiteSpace(value.DeploymentInfoLocation))
                    value.DeploymentInfoLocation = configuration[DeploymentUtils.DeploymentInfoLocationConfigurationKey];
            })
            .Validate(IsValid, "Licensing TrialDays must be from 0 to 90, ExpiringDays from 0 to 3650, and RecoveryDays from 0 to 365.");
        if (configure is not null) options.Configure(configure);

        if (flags is { GlassBreak: true })
        {
            services.TryAddSingleton<IKidaLicenseService>(provider =>
                new GlassBreakLicenseService(product, provider.GetRequiredService<TimeProvider>(), flags.GlassBreakReason));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, KidaLicenseStartupCheck>());
            return services;
        }

        services.TryAddSingleton<KidaLicenseService>();
        services.TryAddSingleton<IKidaLicenseService>(provider => provider.GetRequiredService<KidaLicenseService>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, KidaLicenseStartupCheck>());
        return services;
    }

    private static bool IsValid(KidaLicenseOptions options) =>
        options.TrialDays is >= 0 and <= LicensePolicyLimits.MaximumTrialDays &&
        options.ExpiringDays is >= 0 and <= 3650 &&
        options.RecoveryDays is >= 0 and <= 365;
}
