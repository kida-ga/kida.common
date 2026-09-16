using System.Text;
using System.Text.Json;
using Haley.Models;
using Haley.Services;
using Haley.Utils;

namespace Kida.Licensing.Tests;

public sealed class LicenseRuntimeTests
{
    [Fact]
    public void FirstStartPreparesTheRequestAndTheTrialGrantsEveryFeatureUntilItEnds()
    {
        using var workspace = new LicenseWorkspace();
        using var runtime = workspace.Load();

        var trial = runtime.GetStatus();
        Assert.Equal(LicenseState.Trial, trial.State);
        Assert.True(File.Exists(DeploymentUtils.GetRequestPath(LicenseWorkspace.Product, workspace.Path)));
        Assert.All(LicenseWorkspace.Features, feature => Assert.True(runtime.HasFeature(feature)));
        Assert.True(runtime.TryGetLimit(LicenseWorkspace.TenantMaximum, out var trialMaximum));
        Assert.Equal(5, trialMaximum);
        Assert.Equal(14, trial.DaysRemaining);

        workspace.Clock.Now = workspace.Clock.Now.AddDays(15);
        var ended = runtime.GetStatus();
        Assert.Equal(LicenseState.TrialExpired, ended.State);
        Assert.False(runtime.HasFeature(LicenseWorkspace.Reports));
        Assert.False(runtime.TryGetLimit(LicenseWorkspace.TenantMaximum, out _));
        Assert.StartsWith("The trial has ended", ended.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AHostWillNotStartOnceTheTrialHasEndedAndNoGrantIsInstalled()
    {
        using var workspace = new LicenseWorkspace();
        using (var trial = workspace.Load()) Assert.Equal(LicenseState.Trial, trial.GetStatus().State);

        workspace.Clock.Now = workspace.Clock.Now.AddDays(15);
        var loaded = LicenseRuntime.Load(workspace.Options());

        Assert.False(loaded.Status);
        Assert.Equal("license.unusable", loaded.Key);
        Assert.Contains("grant.trial_expired", loaded.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ASignedGrantMovesThroughExpiringGraceAndExpiryWithoutRereadingItsFile()
    {
        using var workspace = new LicenseWorkspace();
        workspace.InstallLicense(workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), validityDays: 365, graceDays: 10));
        workspace.Clock.Now = new(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        using var runtime = workspace.Load();

        var valid = runtime.GetStatus();
        Assert.Equal(LicenseState.Valid, valid.State);
        Assert.Equal("sample-legacy", valid.LicenseId);
        Assert.True(valid.SignatureValid);
        Assert.True(runtime.HasFeature(LicenseWorkspace.Reports));
        Assert.True(runtime.TryGetLimit(LicenseWorkspace.TenantMaximum, out var maximum));
        Assert.Equal(10, maximum);
        Assert.Equal(214, valid.DaysRemaining);

        File.Delete(workspace.LicensePath);
        workspace.Clock.Now = new(2026, 12, 15, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(LicenseState.Expiring, runtime.GetStatus().State);

        workspace.Clock.Now = new(2027, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var grace = runtime.GetStatus();
        Assert.Equal(LicenseState.Grace, grace.State);
        Assert.True(grace.HasFeature(LicenseWorkspace.Reports));
        Assert.Equal(6, grace.DaysRemaining);

        workspace.Clock.Now = new(2027, 1, 12, 0, 0, 0, TimeSpan.Zero);
        var expired = runtime.GetStatus();
        Assert.Equal(LicenseState.Expired, expired.State);
        Assert.False(expired.TermActive);
        Assert.False(runtime.HasFeature(LicenseWorkspace.Reports));
        Assert.Empty(expired.Limits);
        Assert.Equal("sample-legacy", expired.LicenseId);
    }

    [Fact]
    public async Task PerpetualFeaturesAndEntitlementLimitsRemainAfterTheTermEnds()
    {
        using var workspace = new LicenseWorkspace();
        workspace.InstallLicense(workspace.TermLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), validityDays: 30, graceDays: 0));
        workspace.InstallEntitlement(workspace.Entitlement(term: [LicenseWorkspace.Reports], perpetual: [LicenseWorkspace.Storage]));
        workspace.Clock.Now = new(2026, 1, 10, 0, 0, 0, TimeSpan.Zero);
        using var runtime = workspace.Load();
        Assert.True(runtime.HasFeature(LicenseWorkspace.Reports));
        Assert.True(runtime.HasFeature(LicenseWorkspace.Storage));

        workspace.Clock.Now = new(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        AssertPerpetualOnly(runtime.GetStatus());

        // Evaluated again after expiry, the same files give the same answer.
        var reloaded = await runtime.ReloadAsync();
        Assert.True(reloaded.Status, reloaded.Message);
        AssertPerpetualOnly(reloaded.Result);

        static void AssertPerpetualOnly(LicenseStatus status)
        {
            Assert.Equal(LicenseState.Expired, status.State);
            Assert.False(status.HasFeature(LicenseWorkspace.Reports));
            Assert.True(status.HasFeature(LicenseWorkspace.Storage));
            Assert.True(status.TermFeatures[LicenseWorkspace.Reports]);
            Assert.True(status.PerpetualFeatures[LicenseWorkspace.Storage]);
            Assert.True(status.TryGetLimit(LicenseWorkspace.TenantMaximum, out var maximum));
            Assert.Equal(25, maximum);
            Assert.Equal("The grant and grace period have expired. Only perpetual features remain available.", status.Message);
        }
    }

    [Fact]
    public void ATamperedLicenseDoesNotLetAValidEntitlementThrough()
    {
        using var workspace = new LicenseWorkspace();
        var request = workspace.Request();
        workspace.InstallEntitlement(workspace.Entitlement(term: [LicenseWorkspace.Reports], perpetual: [LicenseWorkspace.Storage], request: request));
        workspace.InstallLicense(LicenseWorkspace.Tamper(workspace.TermLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0)));

        var loaded = LicenseRuntime.Load(workspace.Options());

        Assert.False(loaded.Status);
        Assert.Equal("license.unusable", loaded.Key);
    }

    [Fact]
    public void AnEntitlementCopiedFromAnotherDeploymentIsRejected()
    {
        using var source = new LicenseWorkspace();
        using var target = new LicenseWorkspace();
        target.InstallLicense(target.TermLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0));
        target.InstallEntitlement(source.Entitlement(term: ["*"], perpetual: []));
        using var runtime = target.Load();

        var status = runtime.GetStatus();
        Assert.Equal(LicenseState.Valid, status.State);
        Assert.NotNull(status.FeatureGrantError);
        Assert.All(LicenseWorkspace.Features, feature => Assert.False(status.HasFeature(feature)));
        Assert.Empty(status.Limits);
    }

    [Fact]
    public void AppInfoMustNameTheProductAndAdvertiseExactlyTheCompiledCatalog()
    {
        using (var extraFeature = new LicenseWorkspace([LicenseWorkspace.Reports, LicenseWorkspace.Storage, "anything"]))
        {
            var loaded = LicenseRuntime.Load(extraFeature.Options());
            Assert.False(loaded.Status);
            Assert.Equal("license.appinfo_mismatch", loaded.Key);
        }
        using (var otherProduct = new LicenseWorkspace(product: "another"))
        {
            var loaded = LicenseRuntime.Load(otherProduct.Options());
            Assert.False(loaded.Status);
            Assert.Equal("license.appinfo_mismatch", loaded.Key);
        }
    }

    [Fact]
    public async Task AReplacementIsVerifiedBeforeItIsInstalled()
    {
        using var workspace = new LicenseWorkspace();
        var original = workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0, "sample-original");
        workspace.InstallLicense(original);
        using var runtime = workspace.Load();

        await using (var tampered = Stream(LicenseWorkspace.Tamper(workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0, "sample-tampered"))))
        {
            var refused = await runtime.ReplaceAsync(tampered);
            Assert.False(refused.Status);
            Assert.Equal("license.replacement_invalid", refused.Key);
        }
        await using (var empty = new MemoryStream())
        {
            Assert.Equal("license.artifact_empty", (await runtime.ReplaceAsync(empty)).Key);
        }
        await using (var large = new MemoryStream(new byte[LicenseRuntime.MaxArtifactBytes + 1]))
        {
            Assert.Equal("license.artifact_too_large", (await runtime.ReplaceAsync(large)).Key);
        }
        Assert.Equal(original, File.ReadAllText(workspace.LicensePath));
        Assert.Empty(Directory.GetFiles(workspace.Path, "sample.lic.old-*"));
        Assert.Empty(Directory.GetFiles(workspace.Path, "*.tmp"));

        var replacement = workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0, "sample-renewed");
        await using var stream = Stream(replacement);
        var installed = await runtime.ReplaceAsync(stream);

        Assert.True(installed.Status, installed.Message);
        Assert.Equal("sample-renewed", installed.Result.LicenseId);
        Assert.Equal("sample-renewed", runtime.GetStatus().LicenseId);
        Assert.Equal(replacement, File.ReadAllText(workspace.LicensePath));
        Assert.Single(Directory.GetFiles(workspace.Path, "sample.lic.old-*"));
    }

    [Fact]
    public async Task ReloadKeepsTheActiveLicenseWhenTheInstalledFileBecomesUnusable()
    {
        using var workspace = new LicenseWorkspace();
        var license = workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0, "sample-active");
        workspace.InstallLicense(license);
        using var runtime = workspace.Load();

        workspace.InstallLicense(LicenseWorkspace.Tamper(license));
        var reloaded = await runtime.ReloadAsync();

        Assert.False(reloaded.Status);
        Assert.Equal("license.reload_invalid", reloaded.Key);
        Assert.Equal("sample-active", runtime.GetStatus().LicenseId);
        Assert.True(runtime.HasFeature(LicenseWorkspace.Reports));
    }

    [Fact]
    public void AGrantThatStartsLaterIsEvaluatedAgainOnceItStarts()
    {
        using var workspace = new LicenseWorkspace();
        workspace.Clock.Now = new(2026, 1, 15, 0, 0, 0, TimeSpan.Zero);
        workspace.InstallLicense(workspace.TermLicense(new(2026, 3, 1, 0, 0, 0, TimeSpan.Zero), 365, 0));
        workspace.InstallEntitlement(workspace.Entitlement(term: [LicenseWorkspace.Reports], perpetual: [LicenseWorkspace.Storage]));
        using var runtime = workspace.Load();

        Assert.Equal(LicenseState.NotYetValid, runtime.GetStatus().State);
        Assert.True(runtime.HasFeature(LicenseWorkspace.Storage));
        Assert.False(runtime.HasFeature(LicenseWorkspace.Reports));

        workspace.Clock.Now = new(2026, 3, 2, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(LicenseState.Valid, runtime.GetStatus().State);
        Assert.True(runtime.HasFeature(LicenseWorkspace.Reports));
    }

    [Fact]
    public void TheRequestEnvelopeIsAvailableForTheIssuer()
    {
        using var workspace = new LicenseWorkspace();
        using var runtime = workspace.Load();

        var request = runtime.GetRequest();

        Assert.True(request.Status, request.Message);
        Assert.Equal(File.ReadAllText(DeploymentUtils.GetRequestPath(LicenseWorkspace.Product, workspace.Path)), request.Result);
        using var envelope = JsonDocument.Parse(request.Result);
        Assert.True(envelope.RootElement.TryGetProperty("request", out _));
    }

    private static MemoryStream Stream(string value) => new(Encoding.UTF8.GetBytes(value));
}
