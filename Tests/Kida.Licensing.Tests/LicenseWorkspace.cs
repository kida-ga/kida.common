using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Haley.Models;
using Haley.Services;
using Haley.Utils;

namespace Kida.Licensing.Tests;

/// <summary>A temporary application directory with appinfo.json, an issuer key and helpers to issue grants.</summary>
internal sealed class LicenseWorkspace : IDisposable
{
    internal const string Product = "sample";
    internal const string Version = "1.0.0";
    internal const string Reports = "reports";
    internal const string Storage = "storage";
    internal const string TenantMaximum = "tenant.max";
    internal static readonly string[] Features = [Reports, Storage];
    internal static readonly string[] Limits = [TenantMaximum];
    private readonly RSA _issuer = RSA.Create(2048);

    internal LicenseWorkspace(string[]? appInfoFeatures = null, string product = Product)
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "kida-licensing-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
        File.WriteAllText(System.IO.Path.Combine(Path, "appinfo.json"), JsonSerializer.Serialize(new
        {
            product,
            version = Version,
            features = appInfoFeatures ?? Features,
            limits = new Dictionary<string, object>
            {
                [TenantMaximum] = new { @default = 5, description = "Maximum number of tenants this deployment may onboard." }
            }
        }));
        File.WriteAllText(System.IO.Path.Combine(Path, "issuer.pub"), _issuer.ExportSubjectPublicKeyInfoPem());
    }

    internal string Path { get; }
    internal string LicensePath => System.IO.Path.Combine(Path, "sample.lic");
    internal TestClock Clock { get; } = new();

    internal LicenseRuntimeOptions Options() => new()
    {
        Product = Product,
        Features = Features,
        Limits = Limits,
        BaseDirectory = Path,
        LicensePath = "sample.lic",
        PublicKeyPath = "issuer.pub",
        TrialDays = 14,
        ExpiringDays = 30,
        RecoveryDays = 7,
        Clock = Clock.GetUtcNow
    };

    internal LicenseRuntime Load()
    {
        var loaded = LicenseRuntime.Load(Options());
        Assert.True(loaded.Status, loaded.Key + ": " + loaded.Message);
        return loaded.Result;
    }

    /// <summary>Prepares the deployment request the way product startup would.</summary>
    internal string Request()
    {
        var request = DeploymentUtils.EnsureRequest(new DeploymentRequestInput
        {
            Product = Product,
            ProductVersion = Version,
            Features = Features,
            Limits = LimitCatalog(),
            BaseDirectory = Path,
            LicensePath = "sample.lic",
            NowUtc = Clock.GetUtcNow()
        });
        Assert.True(request.IsValid, request.Error);
        return File.ReadAllText(DeploymentUtils.GetRequestPath(Product, Path));
    }

    /// <summary>A license that carries its own features and limits.</summary>
    internal string LegacyLicense(DateTimeOffset issued, int validityDays, int graceDays, string licenseId = "sample-legacy")
        => DeploymentUtils.PrepareGrant(new DeploymentGrantInput
        {
            RequestEnvelope = Request(),
            LicenseId = licenseId,
            Customer = "Sample customer",
            MachineLock = MachineLockMode.None,
            Features = Features.ToDictionary(item => item, _ => true, StringComparer.Ordinal),
            Limits = new Dictionary<string, JsonElement>(StringComparer.Ordinal) { [TenantMaximum] = JsonSerializer.SerializeToElement(10) },
            IssuedUtc = issued,
            ValidityDays = validityDays,
            GraceDays = graceDays,
            PrivateKey = _issuer.ExportPkcs8PrivateKeyPem(),
            Key = "kida-test"
        });

    /// <summary>A featureless license that owns only the commercial term.</summary>
    internal string TermLicense(DateTimeOffset issued, int validityDays, int graceDays, string licenseId = "sample-term")
        => DeploymentUtils.PrepareLicenseGrant(new DeploymentGrantInput
        {
            RequestEnvelope = Request(),
            LicenseId = licenseId,
            Customer = "Sample customer",
            MachineLock = MachineLockMode.None,
            IssuedUtc = issued,
            ValidityDays = validityDays,
            GraceDays = graceDays,
            PrivateKey = _issuer.ExportPkcs8PrivateKeyPem(),
            Key = "kida-test"
        });

    internal string Entitlement(string[] term, string[] perpetual, long tenantMaximum = 25, string? request = null)
        => DeploymentUtils.PrepareFeatureGrant(new DeploymentFeatureGrantInput
        {
            RequestEnvelope = request ?? Request(),
            GrantId = "sample-features",
            Revision = 1,
            MachineLock = MachineLockMode.None,
            Features = term,
            Perpetual = perpetual,
            Limits = new Dictionary<string, JsonElement>(StringComparer.Ordinal) { [TenantMaximum] = JsonSerializer.SerializeToElement(tenantMaximum) },
            IssuedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            PrivateKey = _issuer.ExportPkcs8PrivateKeyPem(),
            Key = "kida-test"
        });

    internal void InstallLicense(string artifact) => File.WriteAllText(LicensePath, artifact);

    internal void InstallEntitlement(string artifact) => File.WriteAllText(DeploymentUtils.GetFeatureGrantPath(Path), artifact);

    internal static string Tamper(string artifact)
    {
        var envelope = JsonSerializer.Deserialize<Dictionary<string, string>>(artifact)!;
        envelope["payload"] = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"features\":{\"reports\":true}}"));
        return JsonSerializer.Serialize(envelope);
    }

    public void Dispose()
    {
        _issuer.Dispose();
        if (Directory.Exists(Path)) Directory.Delete(Path, recursive: true);
    }

    private static Dictionary<string, DeploymentLimitDefinition> LimitCatalog() => new(StringComparer.Ordinal)
    {
        [TenantMaximum] = new DeploymentLimitDefinition
        {
            DefaultValue = JsonSerializer.SerializeToElement(5),
            Description = "Maximum number of tenants this deployment may onboard."
        }
    };
}

internal sealed class TestClock : TimeProvider
{
    public DateTimeOffset Now { get; set; } = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => Now;
}
