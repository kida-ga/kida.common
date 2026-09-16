using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Kida.Licensing;

/// <summary>Refuses the endpoint with a 403 problem unless every named feature is licensed right now.</summary>
public sealed class KidaLicenseEndpointFilter : IEndpointFilter
{
    private readonly string[] _features;

    public KidaLicenseEndpointFilter(params string[] features)
    {
        ArgumentNullException.ThrowIfNull(features);
        if (features.Length == 0 || features.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Name at least one licensed feature.", nameof(features));
        _features = features;
    }

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        var license = context.HttpContext.RequestServices.GetRequiredService<IKidaLicenseService>();
        foreach (var feature in _features)
        {
            if (!license.HasFeature(feature))
                return ValueTask.FromResult<object?>(KidaLicenseResults.FeatureUnavailable(feature));
        }
        return next(context);
    }
}

public static class KidaLicenseEndpointConventions
{
    /// <summary><c>endpoint.RequireLicensedFeatures("storage.write")</c></summary>
    public static TBuilder RequireLicensedFeatures<TBuilder>(this TBuilder builder, params string[] features)
        where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddEndpointFilter(new KidaLicenseEndpointFilter(features));
    }
}
