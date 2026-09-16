using Microsoft.AspNetCore.Http;

namespace Kida.Licensing;

public static class KidaLicenseResults
{
    public const string FeatureUnavailableCode = "license.feature_unavailable";

    /// <summary>403 problem naming the unlicensed feature, with <c>code</c> and <c>feature</c> extensions.</summary>
    public static IResult FeatureUnavailable(string feature, string? detail = null)
        => Results.Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Feature not licensed",
            detail: detail ?? "This feature is not enabled by the product license.",
            extensions: new Dictionary<string, object?>
            {
                ["code"] = FeatureUnavailableCode,
                ["feature"] = feature
            });
}
