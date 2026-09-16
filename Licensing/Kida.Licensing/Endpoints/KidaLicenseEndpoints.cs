using Haley.Models;
using Haley.Services;
using Haley.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Kida.Licensing;

public static class KidaLicenseEndpoints
{
    /// <summary>
    /// Maps license administration onto a group the host has already secured:
    /// <c>GET ""</c> (status), <c>GET "/request"</c>, <c>POST "/replace"</c> (multipart field <c>license</c>) and
    /// <c>POST "/reload"</c>. Responses are never cached.
    /// </summary>
    public static RouteGroupBuilder MapKidaLicenseEndpoints(this RouteGroupBuilder group, string namePrefix = "Kida")
        => group.MapKidaLicenseEndpoints(static status => status, namePrefix);

    /// <summary>
    /// Maps license administration and shapes the status with <paramref name="project"/>, so a product can keep its
    /// own response contract.
    /// </summary>
    public static RouteGroupBuilder MapKidaLicenseEndpoints<TStatus>(
        this RouteGroupBuilder group,
        Func<LicenseStatus, TStatus> project,
        string namePrefix = "Kida")
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(namePrefix);

        group.MapGet("", (IKidaLicenseService license, HttpContext context) =>
            {
                NoStore(context);
                return Results.Ok(project(license.GetStatus()));
            })
            .WithName($"Get{namePrefix}License")
            .Produces<TStatus>();

        group.MapGet("/request", (IKidaLicenseService license, HttpContext context) =>
            {
                NoStore(context);
                var request = license.GetRequest();
                return request.Status ? Results.Ok(new KidaDeploymentRequest(request.Result)) : request.ToMinimalApiResult();
            })
            .WithName($"Get{namePrefix}DeploymentRequest")
            .Produces<KidaDeploymentRequest>();

        group.MapPost("/replace", async (HttpRequest request, IKidaLicenseService license, CancellationToken cancellationToken) =>
            {
                NoStore(request.HttpContext);
                if (!request.HasFormContentType)
                    return Refuse("license.artifact_required", "Upload the license artifact as multipart form data.");
                var form = await request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
                var file = form.Files.GetFile("license") ?? (form.Files.Count > 0 ? form.Files[0] : null);
                if (file is null) return Refuse("license.artifact_required", "A license artifact is required.");
                if (file.Length > LicenseRuntime.MaxArtifactBytes)
                    return Refuse("license.artifact_too_large", "The license artifact exceeds 256 KB.");

                await using var stream = file.OpenReadStream();
                var result = await license.ReplaceAsync(stream, cancellationToken).ConfigureAwait(false);
                return result.Status ? Results.Ok(project(result.Result)) : result.ToMinimalApiResult();
            })
            .WithName($"Replace{namePrefix}License")
            .Produces<TStatus>();

        group.MapPost("/reload", async (IKidaLicenseService license, HttpContext context, CancellationToken cancellationToken) =>
            {
                NoStore(context);
                var result = await license.ReloadAsync(cancellationToken).ConfigureAwait(false);
                return result.Status ? Results.Ok(project(result.Result)) : result.ToMinimalApiResult();
            })
            .WithName($"Reload{namePrefix}License")
            .Produces<TStatus>();

        return group;
    }

    private static void NoStore(HttpContext context) => context.Response.Headers.CacheControl = "no-store";

    private static IResult Refuse(string key, string message)
        => new Feedback<LicenseStatus>(false, message).SetKey(key).SetCode(StatusCodes.Status400BadRequest)
            .SetSource(LicenseRuntime.FeedbackSource).ToMinimalApiResult();
}
