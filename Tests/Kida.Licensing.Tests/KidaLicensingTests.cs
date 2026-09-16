using System.Security.Claims;
using Haley.Abstractions;
using Haley.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kida.Licensing.Tests;

public sealed class KidaLicensingTests
{
    private static readonly KidaLicenseProduct Product = new(LicenseWorkspace.Product, LicenseWorkspace.Features, LicenseWorkspace.Limits);

    [Fact]
    public void DefaultRegistrationUsesKidaOwnedDefaultsAndOverrides()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Kida:Licensing:TrialDays"] = "180",
            ["Kida:Licensing:WarnGlassBreak"] = "false"
        }).Build();

        using var services = new ServiceCollection().AddLogging()
            .AddKidaLicensing(configuration, Product)
            .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<KidaLicenseOptions>>().Value;
        Assert.Null(options.Path);
        Assert.Null(options.PublicKeyPath);
        Assert.Equal(LicensePolicyLimits.MaximumTrialDays, options.TrialDays);
        Assert.Equal(30, options.ExpiringDays);
        Assert.Equal(7, options.RecoveryDays);
        Assert.False(options.WarnGlassBreak);
    }

    [Fact]
    public void OneRegistrationLoadsTheLicenseFromTheContentRootAndConfiguration()
    {
        using var workspace = new LicenseWorkspace();
        workspace.InstallLicense(workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0, "sample-registered"));
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Sample:Licensing:Path"] = "sample.lic",
            ["Sample:Licensing:PublicKeyPath"] = "issuer.pub"
        }).Build();

        using var services = new ServiceCollection().AddLogging()
            .AddSingleton<IHostEnvironment>(new TestEnvironment(workspace.Path))
            .AddSingleton<TimeProvider>(workspace.Clock)
            .AddKidaLicensing(configuration, Product, "Sample:Licensing")
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        var license = services.GetRequiredService<IKidaLicenseService>();
        Assert.Same(services.GetRequiredService<KidaLicenseService>(), license);
        Assert.Equal("sample-registered", license.GetStatus().LicenseId);
        Assert.True(license.HasFeature(LicenseWorkspace.Reports));
        Assert.Contains(services.GetServices<IHostedService>(), service => service is KidaLicenseStartupCheck);
    }

    [Fact]
    public void AnUnusableLicenseStopsTheHostWithItsReason()
    {
        using var workspace = new LicenseWorkspace();
        workspace.InstallLicense(LicenseWorkspace.Tamper(workspace.LegacyLicense(new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 365, 0)));
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Sample:Licensing:Path"] = "sample.lic",
            ["Sample:Licensing:PublicKeyPath"] = "issuer.pub"
        }).Build();
        using var services = new ServiceCollection().AddLogging()
            .AddSingleton<IHostEnvironment>(new TestEnvironment(workspace.Path))
            .AddSingleton<TimeProvider>(workspace.Clock)
            .AddKidaLicensing(configuration, Product, "Sample:Licensing")
            .BuildServiceProvider();

        var exception = Assert.Throws<InvalidOperationException>(() => services.GetRequiredService<IKidaLicenseService>());

        Assert.Contains("license.unusable", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AdministrationMapsFourEndpointsOntoTheHostsSecuredGroup()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<IKidaLicenseService>(new FakeLicense());
        using var app = builder.Build();
        app.MapGroup("/api/sample/license").RequireAuthorization("Sample.ManageLicense").MapKidaLicenseEndpoints("Sample");

        var routes = ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints).OfType<RouteEndpoint>().ToArray();

        Assert.Equal(
            ["GetSampleLicense", "GetSampleDeploymentRequest", "ReplaceSampleLicense", "ReloadSampleLicense"],
            routes.Select(route => route.Metadata.GetMetadata<IEndpointNameMetadata>()!.EndpointName));
        Assert.All(routes, route => Assert.Contains(
            route.Metadata.GetOrderedMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>(),
            data => data.Policy == "Sample.ManageLicense"));
    }

    [Fact]
    public async Task TheEndpointFilterRefusesAnUnlicensedFeatureWithAProblem()
    {
        using var services = new ServiceCollection().AddLogging().AddSingleton<IKidaLicenseService>(new FakeLicense()).BuildServiceProvider();
        var refusedContext = Context(services);
        var invoked = false;
        EndpointFilterDelegate next = _ => { invoked = true; return ValueTask.FromResult<object?>(Results.Ok()); };

        var refused = await new KidaLicenseEndpointFilter(LicenseWorkspace.Storage).InvokeAsync(new DefaultEndpointFilterInvocationContext(refusedContext), next);
        await Assert.IsAssignableFrom<IResult>(refused).ExecuteAsync(refusedContext);

        Assert.False(invoked);
        Assert.Equal(StatusCodes.Status403Forbidden, refusedContext.Response.StatusCode);
        refusedContext.Response.Body.Position = 0;
        var body = await new StreamReader(refusedContext.Response.Body).ReadToEndAsync();
        Assert.Contains(KidaLicenseResults.FeatureUnavailableCode, body, StringComparison.Ordinal);
        Assert.Contains(LicenseWorkspace.Storage, body, StringComparison.Ordinal);

        await new KidaLicenseEndpointFilter(LicenseWorkspace.Reports).InvokeAsync(new DefaultEndpointFilterInvocationContext(Context(services)), next);
        Assert.True(invoked);
    }

    private static DefaultHttpContext Context(IServiceProvider services) => new()
    {
        RequestServices = services,
        User = new ClaimsPrincipal(new ClaimsIdentity()),
        Response = { Body = new MemoryStream() }
    };

    private sealed class TestEnvironment(string contentRoot) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "Kida.Licensing.Tests";
        public string ContentRootPath { get; set; } = contentRoot;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class FakeLicense : IKidaLicenseService
    {
        public string Product => LicenseWorkspace.Product;
        public LicenseStatus GetStatus() => new() { State = LicenseState.Valid };
        public bool HasFeature(string feature) => feature == LicenseWorkspace.Reports;
        public bool TryGetLimit(string code, out long value) { value = 0; return false; }
        public IFeedback<string> GetRequest() => new Feedback<string>(false, "unavailable");
        public Task<IFeedback<LicenseStatus>> ReplaceAsync(Stream artifact, CancellationToken cancellationToken = default)
            => Task.FromResult<IFeedback<LicenseStatus>>(new Feedback<LicenseStatus>(false, "refused"));
        public Task<IFeedback<LicenseStatus>> ReloadAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IFeedback<LicenseStatus>>(new Feedback<LicenseStatus>(false, "refused"));
    }
}
