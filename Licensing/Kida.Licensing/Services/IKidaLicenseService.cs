using Haley.Abstractions;
using Haley.Models;

namespace Kida.Licensing;

/// <summary>
/// The product's loaded deployment license. Feature and limit checks are answered from memory against the current
/// time; administration operations return Haley feedback with a stable key.
/// </summary>
public interface IKidaLicenseService
{
    string Product { get; }

    /// <summary>The license as it stands now. Safe to show to administrators.</summary>
    LicenseStatus GetStatus();

    bool HasFeature(string feature);

    bool TryGetLimit(string code, out long value);

    /// <summary>The deployment request envelope to send to the issuer.</summary>
    IFeedback<string> GetRequest();

    /// <summary>Verifies and installs a replacement license; the active license is untouched when it is refused.</summary>
    Task<IFeedback<LicenseStatus>> ReplaceAsync(Stream artifact, CancellationToken cancellationToken = default);

    /// <summary>Evaluates the installed license files again.</summary>
    Task<IFeedback<LicenseStatus>> ReloadAsync(CancellationToken cancellationToken = default);
}
