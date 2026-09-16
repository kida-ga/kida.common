using Haley.Models;
using Haley.Utils;
using Kida.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Kida.Service.Client;

internal sealed class ApplicationInfoProductCatalogProvider(
    IOptions<KidaProductCatalogOptions> catalogOptions,
    IOptions<KidaClientOptions> clientOptions) : IKidaProductCatalogProvider
{
    public ValueTask<RegisterEntitlementCatalogRequest> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var options = catalogOptions.Value;
        var baseDirectory = string.IsNullOrWhiteSpace(options.BaseDirectory) ? AppContext.BaseDirectory : Path.GetFullPath(options.BaseDirectory);
        var appInfoPath = Path.GetFullPath(Path.IsPathRooted(options.ApplicationInfoPath)
            ? options.ApplicationInfoPath
            : Path.Combine(baseDirectory, options.ApplicationInfoPath));
        var application = DeploymentUtils.ReadApplicationInfo(appInfoPath);
        var deployment = DeploymentUtils.EnsureRequest(new DeploymentRequestInput
        {
            Product = application.Product,
            ProductVersion = application.ProductVersion,
            Features = application.Features,
            Limits = application.Limits,
            BaseDirectory = baseDirectory,
            DeploymentInfoLocation = options.DeploymentInfoLocation
        });
        if (!deployment.IsValid || deployment.Request is null || !Guid.TryParseExact(deployment.Request.DeployId, "N", out var deploymentId))
        {
            throw new InvalidOperationException($"The Kida product catalog could not establish the deployment identity ({deployment.Error ?? "request.invalid"}).");
        }

        var declaredFeatures = new HashSet<string>(application.Features, StringComparer.Ordinal);
        var duplicateLimit = application.Limits.Keys.FirstOrDefault(declaredFeatures.Contains);
        if (duplicateLimit is not null)
        {
            throw new InvalidOperationException(
                $"The application information declares '{duplicateLimit}' as both a feature and a limit.");
        }

        var features = application.Features
            .Select(static code => new EntitlementFeatureDeclaration(code, code, null, "boolean", 0))
            .Concat(application.Limits.Select(static item => new EntitlementFeatureDeclaration(
                item.Key,
                item.Key,
                string.IsNullOrWhiteSpace(item.Value.Description) ? null : item.Value.Description.Trim(),
                ValueType(item.Value.DefaultValue),
                0)))
            .OrderBy(static feature => feature.Code, StringComparer.Ordinal)
            .ToArray();
        return ValueTask.FromResult(new RegisterEntitlementCatalogRequest(
            deploymentId,
            application.Product,
            string.IsNullOrWhiteSpace(options.DisplayName) ? application.Product : options.DisplayName.Trim(),
            string.IsNullOrWhiteSpace(options.Description) ? null : options.Description.Trim(),
            clientOptions.Value.UserAudience,
            application.ProductVersion,
            features,
            Array.Empty<EntitlementMeterDeclaration>()));
    }

    private static string ValueType(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.True or JsonValueKind.False => "boolean",
        JsonValueKind.Number when value.TryGetInt64(out _) => "integer",
        JsonValueKind.Number => "decimal",
        JsonValueKind.String => "text",
        _ => "json"
    };
}
