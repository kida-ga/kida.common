namespace Kida.Licensing;

/// <summary>Deployment settings bound from the product's licensing configuration section.</summary>
public sealed class KidaLicenseOptions
{
    /// <summary>License artifact path. Empty uses <c>license.lic</c> in the deployment-information directory.</summary>
    public string? Path { get; set; }

    /// <summary>Issuer public key path. Empty uses Haley's trusted key registry.</summary>
    public string? PublicKeyPath { get; set; }

    public int TrialDays { get; set; } = 14;
    public int ExpiringDays { get; set; } = 30;
    public int RecoveryDays { get; set; } = 7;

    /// <summary>
    /// Whether a host should visibly announce a deliberate code-level glass break. This setting cannot enable
    /// glass break; it only controls presentation after the host has explicitly broken the glass in code.
    /// </summary>
    public bool WarnGlassBreak { get; set; } = true;

    /// <summary>Application base directory. Empty uses the host content root.</summary>
    public string? BaseDirectory { get; set; }

    /// <summary>Path to <c>appinfo.json</c>. Empty uses <c>appinfo.json</c> in the base directory.</summary>
    public string? ApplicationInfoPath { get; set; }

    /// <summary>Deployment-information directory. Empty uses the top-level <c>deployinfo-location</c> setting.</summary>
    public string? DeploymentInfoLocation { get; set; }
}
