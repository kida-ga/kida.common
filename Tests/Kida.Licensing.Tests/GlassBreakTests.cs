using Haley.Models;
using Haley.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kida.Licensing.Tests;

public sealed class GlassBreakTests
{
    private static readonly KidaLicenseProduct Product = new(LicenseWorkspace.Product, LicenseWorkspace.Features, LicenseWorkspace.Limits);

    [Fact]
    public void ABrokenGlassGrantsEveryFeatureAndNeverTouchesTheLicenceFiles()
    {
        using var workspace = new LicenseWorkspace();
        // Even an unusable licence, which normally stops the host, is not read at all.
        workspace.InstallLicense(LicenseWorkspace.Tamper(workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0)));
        File.Delete(DeploymentUtils.GetRequestPath(LicenseWorkspace.Product, workspace.Path));

        using var services = Services(workspace, new AppFlags().BreakGlass("Issuer unreachable during migration"));
        var license = services.GetRequiredService<IKidaLicenseService>();
        var status = license.GetStatus();

        Assert.Equal(LicenseState.GlassBreak, status.State);
        Assert.All(LicenseWorkspace.Features, feature => Assert.True(license.HasFeature(feature)));
        Assert.False(license.HasFeature("anything-else"));
        Assert.False(license.TryGetLimit(LicenseWorkspace.TenantMaximum, out _));
        Assert.Contains("Issuer unreachable during migration", status.Message, StringComparison.Ordinal);
        Assert.False(File.Exists(DeploymentUtils.GetRequestPath(LicenseWorkspace.Product, workspace.Path)));
    }

    [Fact]
    public async Task ABrokenGlassOffersNoDeploymentRequestOrLicenceAdministration()
    {
        using var workspace = new LicenseWorkspace();
        using var services = Services(workspace, new AppFlags().BreakGlass("Emergency"));
        var license = services.GetRequiredService<IKidaLicenseService>();

        await using var artifact = new MemoryStream([1, 2, 3]);
        foreach (var key in new[] { license.GetRequest().Key, (await license.ReplaceAsync(artifact)).Key, (await license.ReloadAsync()).Key })
            Assert.Equal("license.glass_break", key);
    }

    [Fact]
    public void TheGlassCanOnlyBeBrokenInCode()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AppFlags"] = "debug,glassbreak,glass-break",
            ["AppFlags:GlassBreak"] = "true",
            ["GlassBreak"] = "true"
        }).Build();

        var bound = new AppFlags();
        configuration.Bind(bound);
        configuration.GetSection("AppFlags").Bind(bound);
        bound.LoadFromConfig(configuration);

        Assert.False(bound.GlassBreak);
        Assert.True(bound.Debug); // the rest of the flags still load from configuration
        Assert.True(new AppFlags().BreakGlass("Stated reason").GlassBreak);
        Assert.Throws<ArgumentException>(() => new AppFlags().BreakGlass("  "));
    }

    [Fact]
    public void TheHostRecordsThatLicensingIsSwitchedOff()
    {
        using var workspace = new LicenseWorkspace();
        using var services = Services(workspace, new AppFlags().BreakGlass("Recorded reason"));

        var startup = Assert.Single(services.GetServices<IHostedService>());

        Assert.IsType<KidaLicenseStartupCheck>(startup);
    }

    private static ServiceProvider Services(LicenseWorkspace workspace, AppFlags flags)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Sample:Licensing:Path"] = "sample.lic",
            ["Sample:Licensing:PublicKeyPath"] = "issuer.pub"
        }).Build();
        return new ServiceCollection().AddLogging()
            .AddSingleton<TimeProvider>(workspace.Clock)
            .AddKidaLicensing(configuration, Product, "Sample:Licensing", flags)
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}
