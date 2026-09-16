using Haley.Models;
using Haley.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Kida.Licensing;

public static class KidaLicensingRegistration
{
    /// <summary>
    /// Registers the product license: <c>services.AddKidaLicensing(configuration, product, "Product:Licensing")</c>.
    /// The license is loaded when the host starts, and startup fails when it cannot be used.
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
        if (flags is { GlassBreak: true })
        {
            services.TryAddSingleton<IKidaLicenseService>(provider =>
                new GlassBreakLicenseService(product, provider.GetRequiredService<TimeProvider>(), flags.GlassBreakReason));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, KidaLicenseStartupCheck>());
            return services;
        }

        var options = services.AddOptions<KidaLicenseOptions>()
            .Bind(configuration.GetSection(sectionName))
            .PostConfigure(value =>
            {
                if (string.IsNullOrWhiteSpace(value.DeploymentInfoLocation))
                    value.DeploymentInfoLocation = configuration[DeploymentUtils.DeploymentInfoLocationConfigurationKey];
            })
            .Validate(IsValid, "Licensing TrialDays and ExpiringDays must be between 0 and 3650, and RecoveryDays between 0 and 365.");
        if (configure is not null) options.Configure(configure);

        services.TryAddSingleton<KidaLicenseService>();
        services.TryAddSingleton<IKidaLicenseService>(provider => provider.GetRequiredService<KidaLicenseService>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, KidaLicenseStartupCheck>());
        return services;
    }

    private static bool IsValid(KidaLicenseOptions options) =>
        options.TrialDays is >= 0 and <= 3650 &&
        options.ExpiringDays is >= 0 and <= 3650 &&
        options.RecoveryDays is >= 0 and <= 365;
}
