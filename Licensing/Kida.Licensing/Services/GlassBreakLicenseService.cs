using Haley.Abstractions;
using Haley.Models;
using Haley.Services;

namespace Kida.Licensing;

/// <summary>
/// Licensing deliberately switched off for this deployment by a hardcoded <see cref="AppFlags.BreakGlass"/>.
/// No licence file is read, so a missing, lost or expired grant cannot stop the host, and every compiled feature
/// is available. Limits are not enforced. The deployment request and licence administration are unavailable until
/// the glass break is removed from the code.
/// </summary>
public sealed class GlassBreakLicenseService : IKidaLicenseService
{
    private readonly KidaLicenseProduct _product;
    private readonly TimeProvider _clock;
    private readonly string _reason;

    public GlassBreakLicenseService(KidaLicenseProduct product, TimeProvider clock, string reason)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(clock);
        _product = product;
        _clock = clock;
        _reason = string.IsNullOrWhiteSpace(reason) ? "No reason given." : reason.Trim();
    }

    public string Product => _product.Product;

    public LicenseStatus GetStatus() => new()
    {
        State = LicenseState.GlassBreak,
        CheckedUtc = _clock.GetUtcNow(),
        Product = _product.Product,
        TermActive = true,
        Features = All(),
        TermFeatures = All(),
        PerpetualFeatures = All(),
        Message = "Licensing is switched off for this deployment (glass break): " + _reason
    };

    public bool HasFeature(string feature) => !string.IsNullOrWhiteSpace(feature) && _product.Features.Contains(feature);

    public bool TryGetLimit(string code, out long value)
    {
        value = 0;
        return false; // No grant, no limits: the product's limit checks do not apply.
    }

    public IFeedback<string> GetRequest() => Unavailable<string>();

    public Task<IFeedback<LicenseStatus>> ReplaceAsync(Stream artifact, CancellationToken cancellationToken = default)
        => Task.FromResult(Unavailable<LicenseStatus>());

    public Task<IFeedback<LicenseStatus>> ReloadAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Unavailable<LicenseStatus>());

    private Dictionary<string, bool> All()
        => _product.Features.ToDictionary(feature => feature, _ => true, StringComparer.Ordinal);

    private static IFeedback<T> Unavailable<T>()
        => new Feedback<T>(false, "Licensing is switched off for this deployment (glass break). Remove the glass break from the host code to install or inspect a licence.")
            .SetKey("license.glass_break").SetCode(409).SetSource(LicenseRuntime.FeedbackSource);
}
