using Haley.Abstractions;
using Haley.Models;
using Haley.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kida.Licensing;

/// <summary>
/// Loads the product license when it is first resolved and fails closed: an unreadable or mismatched
/// <c>appinfo.json</c>, an unpreparable deployment request, or an unusable grant stops the host.
/// </summary>
public sealed class KidaLicenseService : IKidaLicenseService, IDisposable
{
    private static readonly Action<ILogger, string, string, Exception?> Loaded = LoggerMessage.Define<string, string>(
        LogLevel.Information, new EventId(4601, "DeploymentGrantLoaded"), "The {Product} deployment grant is {State}.");
    private static readonly Action<ILogger, string, string, string, Exception?> Invalid = LoggerMessage.Define<string, string, string>(
        LogLevel.Critical, new EventId(4602, "DeploymentGrantInvalid"), "The {Product} license could not be loaded ({Key}): {Reason}");
    private static readonly Action<ILogger, string, string, Exception?> Replaced = LoggerMessage.Define<string, string>(
        LogLevel.Information, new EventId(4603, "DeploymentGrantReplaced"), "The {Product} deployment grant was replaced; it is {State}.");
    private static readonly Action<ILogger, string, string, Exception?> Reloaded = LoggerMessage.Define<string, string>(
        LogLevel.Information, new EventId(4604, "DeploymentGrantReloaded"), "The {Product} deployment grant was reloaded; it is {State}.");
    private static readonly Action<ILogger, string, string, string, Exception?> Refused = LoggerMessage.Define<string, string, string>(
        LogLevel.Warning, new EventId(4605, "DeploymentGrantRefused"), "A {Product} license operation was refused ({Key}): {Reason}");
    private readonly LicenseRuntime _runtime;
    private readonly ILogger<KidaLicenseService> _logger;

    public KidaLicenseService(
        KidaLicenseProduct product,
        IOptions<KidaLicenseOptions> options,
        TimeProvider clock,
        ILogger<KidaLicenseService> logger,
        IHostEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(clock);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var settings = options.Value;
        Product = product.Product;
        var loaded = LicenseRuntime.Load(new LicenseRuntimeOptions
        {
            Product = product.Product,
            Features = product.Features,
            Limits = product.Limits,
            BaseDirectory = string.IsNullOrWhiteSpace(settings.BaseDirectory) ? environment?.ContentRootPath : settings.BaseDirectory,
            ApplicationInfoPath = settings.ApplicationInfoPath,
            DeploymentInfoLocation = settings.DeploymentInfoLocation,
            LicensePath = settings.Path,
            PublicKeyPath = settings.PublicKeyPath,
            TrialDays = settings.TrialDays,
            ExpiringDays = settings.ExpiringDays,
            RecoveryDays = settings.RecoveryDays,
            Clock = clock.GetUtcNow
        });
        if (!loaded.Status || loaded.Result is null)
        {
            Invalid(_logger, Product, loaded.Key, loaded.Message, null);
            throw new InvalidOperationException($"The {Product} license could not be loaded ({loaded.Key}): {loaded.Message}");
        }
        _runtime = loaded.Result;
        Loaded(_logger, Product, _runtime.Current.GetState(_runtime.Now).ToString(), null);
    }

    public string Product { get; }

    public LicenseStatus GetStatus() => _runtime.GetStatus();

    public bool HasFeature(string feature) => _runtime.HasFeature(feature);

    public bool TryGetLimit(string code, out long value) => _runtime.TryGetLimit(code, out value);

    public IFeedback<string> GetRequest() => Report(_runtime.GetRequest());

    public async Task<IFeedback<LicenseStatus>> ReplaceAsync(Stream artifact, CancellationToken cancellationToken = default)
    {
        var result = Report(await _runtime.ReplaceAsync(artifact, cancellationToken).ConfigureAwait(false));
        if (result.Status) Replaced(_logger, Product, result.Result.State.ToString(), null);
        return result;
    }

    public async Task<IFeedback<LicenseStatus>> ReloadAsync(CancellationToken cancellationToken = default)
    {
        var result = Report(await _runtime.ReloadAsync(cancellationToken).ConfigureAwait(false));
        if (result.Status) Reloaded(_logger, Product, result.Result.State.ToString(), null);
        return result;
    }

    public void Dispose() => _runtime.Dispose();

    private IFeedback<T> Report<T>(IFeedback<T> result)
    {
        if (!result.Status) Refused(_logger, Product, result.Key, result.Message, null);
        return result;
    }
}
