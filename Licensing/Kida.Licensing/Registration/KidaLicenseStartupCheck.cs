using Haley.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kida.Licensing;

/// <summary>
/// Resolves the license as the host starts, so an unusable license stops startup instead of the first request,
/// and records once when licensing is switched off by a glass break.
/// </summary>
internal sealed class KidaLicenseStartupCheck(IServiceProvider services, ILogger<KidaLicenseStartupCheck> logger) : IHostedService
{
    private static readonly Action<ILogger, string, string, Exception?> GlassBroken = LoggerMessage.Define<string, string>(
        LogLevel.Critical, new EventId(4606, "LicenseGlassBreak"),
        "Licensing is switched off for {Product} by a hardcoded glass break. {Reason}");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var license = services.GetRequiredService<IKidaLicenseService>();
        var status = license.GetStatus();
        if (status.State == LicenseState.GlassBreak) GlassBroken(logger, license.Product, status.Message ?? string.Empty, null);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
