using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Kida.Constants;
using Kida.Models;
using Kida.Service.Client;
using Kida.Utils;
using Haley.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kida.Extensions;

public static class KidaAuthEdgeEndpoints
{
    public static WebApplication MapKidaAuthEdge(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var edge = app.Services.GetRequiredService<IOptions<KidaAuthEdgeOptions>>().Value;
        var client = app.Services.GetRequiredService<IOptions<KidaClientOptions>>().Value;
        if (!KidaResourceAudience.TryNormalize(client.UserAudience, out var audience) ||
            KidaInternalSecurityBoundary.IsInternalAudience(audience))
        {
            throw new InvalidOperationException(
                "A product-hosted Kida Auth Edge requires one fixed non-Kida UserAudience. " +
                "The reserved kida-service audience can never be exposed through a product edge.");
        }

        app.UseRateLimiter();
        var apiPrefix = NormalizePrefix(edge.RoutePrefix);
        var browserPrefix = NormalizePrefix(edge.BrowserRoutePrefix);
        var group = app.MapGroup(apiPrefix).WithTags("Kida Product Auth Edge");

        if (edge.MapClientTokenExchange)
        {
            group.MapPost("/client-tokens", IssueClientTokenAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.ClientToken)
                .AllowAnonymous();
        }

        if (edge.MapRawSessionEndpoints)
        {
            group.MapPost("/sessions/login", AuthenticateAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Authentication)
                .AllowAnonymous();
            group.MapPost("/sessions/refresh", RefreshAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Authentication)
                .AllowAnonymous();
            group.MapDelete("/sessions/{sessionId:guid}", RevokeSessionAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Authentication)
                .AllowAnonymous();
        }

        if (edge.MapPasswordCeremonies)
        {
            group.MapPost("/password/change", ChangePasswordAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
            group.MapPost("/password/resets/verify", VerifyPasswordResetAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
            group.MapPost("/password/resets/complete", CompletePasswordResetAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
        }

        if (edge.MapOnboardingCeremonies)
        {
            group.MapPost("/verifications/complete", VerifyInvitationAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
            group.MapPost("/enrollments/password", CompleteEnrollmentAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
        }

        if (edge.MapMfaCeremonies)
        {
            group.MapGet("/mfa/methods", ListMfaMethodsAsync).AllowAnonymous();
            group.MapPost("/mfa/enrollments", BeginMfaEnrollmentAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
            group.MapPost("/mfa/recovery-codes", ReplaceRecoveryCodesAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
        }

        if (edge.MapSamlCeremonies)
        {
            group.MapPost("/saml/start", BeginSamlAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Federation)
                .AllowAnonymous();
            group.MapPost("/saml/acs", CompleteSamlAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Federation)
                .AllowAnonymous();
        }

        MapBrowserEndpoints(app, apiPrefix, browserPrefix, edge);
        return app;
    }

    private static void MapBrowserEndpoints(
        IEndpointRouteBuilder endpoints,
        string apiPrefix,
        string browserPrefix,
        KidaAuthEdgeOptions options)
    {
        var browser = endpoints.MapGroup(browserPrefix).WithTags("Kida Product Auth Browser");
        var minimumLength = options.MinimumPasswordLength.ToString(
            System.Globalization.CultureInfo.InvariantCulture);

        if (options.MapPasswordCeremonies)
        {
            browser.MapGet("/password/change", (HttpContext context) =>
            {
                SetBrowserHeaders(context);
                return Results.Content(
                    KidaAuthEdgeBrowserResources.PasswordChangeHtml
                        .Replace("{{API_PREFIX}}", WebUtility.HtmlEncode(apiPrefix), StringComparison.Ordinal)
                        .Replace("{{BROWSER_PREFIX}}", WebUtility.HtmlEncode(browserPrefix), StringComparison.Ordinal)
                        .Replace("{{MINIMUM_PASSWORD_LENGTH}}", minimumLength, StringComparison.Ordinal),
                    "text/html; charset=utf-8");
            }).AllowAnonymous();
            browser.MapGet("/password/reset", (HttpContext context) =>
            {
                SetBrowserHeaders(context);
                return Results.Content(
                    KidaAuthEdgeBrowserResources.PasswordResetHtml
                        .Replace("{{API_PREFIX}}", WebUtility.HtmlEncode(apiPrefix), StringComparison.Ordinal)
                        .Replace("{{BROWSER_PREFIX}}", WebUtility.HtmlEncode(browserPrefix), StringComparison.Ordinal)
                        .Replace("{{MINIMUM_PASSWORD_LENGTH}}", minimumLength, StringComparison.Ordinal),
                    "text/html; charset=utf-8");
            }).AllowAnonymous();
            browser.MapGet("/assets/password-change.css", () =>
                Results.Content(KidaAuthEdgeBrowserResources.PasswordChangeCss, "text/css; charset=utf-8")).AllowAnonymous();
            browser.MapGet("/assets/password-change.js", () =>
                Results.Content(
                    KidaAuthEdgeBrowserResources.PasswordChangeJavaScript.Replace(
                        "{{MINIMUM_PASSWORD_LENGTH}}",
                        minimumLength,
                        StringComparison.Ordinal),
                    "text/javascript; charset=utf-8")).AllowAnonymous();
            browser.MapGet("/assets/password-reset.css", () =>
                Results.Content(KidaAuthEdgeBrowserResources.PasswordResetCss, "text/css; charset=utf-8")).AllowAnonymous();
            browser.MapGet("/assets/password-reset.js", () =>
                Results.Content(
                    KidaAuthEdgeBrowserResources.PasswordResetJavaScript.Replace(
                        "{{MINIMUM_PASSWORD_LENGTH}}",
                        minimumLength,
                        StringComparison.Ordinal),
                    "text/javascript; charset=utf-8")).AllowAnonymous();
        }

        if (options.MapMfaCeremonies)
        {
            browser.MapGet("/mfa/enroll", GetMfaEnrollmentAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
            browser.MapPost("/mfa/enroll", ConfirmMfaEnrollmentAsync)
                .RequireRateLimiting(KidaAuthEdgeRateLimits.Password)
                .AllowAnonymous();
            browser.MapGet("/assets/mfa-enroll.css", () =>
                Results.Content(KidaAuthEdgeBrowserResources.MfaEnrollmentCss, "text/css; charset=utf-8")).AllowAnonymous();
        }
    }

    private static async Task<IResult> IssueClientTokenAsync(
        [FromBody] KidaAuthEdgeClientTokenRequest request,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await edge.IssueProductClientTokenAsync(
                request.ClientIdentifier,
                request.ClientSecret,
                cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> AuthenticateAsync(
        [FromBody] AuthenticateRequest request,
        HttpContext context,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        try
        {
            var bound = request with
            {
                ClientId = null,
                Resource = null,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers.UserAgent.ToString()
            };
            return Results.Ok(await kida.AuthenticateAsync(bound, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> RefreshAsync(
        [FromBody] KidaAuthEdgeRefreshRequest request,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await kida.RefreshSessionAsync(request.RefreshToken, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> RevokeSessionAsync(
        Guid sessionId,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        try
        {
            await kida.RevokeSessionAsync(sessionId, cancellationToken).ConfigureAwait(false);
            return Results.NoContent();
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        try
        {
            var receipt = await edge.ChangePasswordAtEdgeAsync(request, cancellationToken).ConfigureAwait(false);
            return receipt is null ? Results.NoContent() : Results.Ok(receipt);
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> VerifyPasswordResetAsync(
        [FromBody] KidaAuthEdgePasswordResetVerifyRequest request,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await edge.VerifyPasswordResetAsync(
                request.ChallengeId,
                request.Code,
                request.ReturnUri,
                request.State,
                cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> CompletePasswordResetAsync(
        [FromBody] KidaAuthEdgePasswordResetCompleteRequest request,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await edge.CompletePasswordResetAsync(
                request.GrantId,
                request.NewPassword,
                request.ReturnUri,
                request.State,
                cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> VerifyInvitationAsync(
        [FromBody] VerifyIdentityInvitationRequest request,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await kida.VerifyInvitationAsync(request, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> CompleteEnrollmentAsync(
        [FromBody] CompleteIdentityEnrollmentRequest request,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await kida.CompleteEnrollmentAsync(request, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> ListMfaMethodsAsync(
        HttpContext context,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        if (!TryGetBearer(context, out var token)) return Results.Unauthorized();
        try
        {
            return Results.Ok(await kida.ListMfaMethodsAsync(token, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> BeginMfaEnrollmentAsync(
        [FromBody] BeginTotpEnrollmentRequest request,
        HttpContext context,
        [FromServices] IKidaClient kida,
        [FromServices] IOptions<KidaAuthEdgeOptions> options,
        CancellationToken cancellationToken)
    {
        if (!TryGetBearer(context, out var token)) return Results.Unauthorized();
        try
        {
            var result = await kida.BeginTotpEnrollmentAsync(
                token,
                request with { ClientId = null, Audience = null },
                cancellationToken).ConfigureAwait(false);
            var path = $"{NormalizePrefix(options.Value.BrowserRoutePrefix)}/mfa/enroll?ticket={Uri.EscapeDataString(result.Ticket)}";
            return Results.Ok(result with { BrowserPath = path });
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> ReplaceRecoveryCodesAsync(
        [FromBody] ReplaceRecoveryCodesRequest request,
        HttpContext context,
        [FromServices] IKidaClient kida,
        CancellationToken cancellationToken)
    {
        if (!TryGetBearer(context, out var token)) return Results.Unauthorized();
        try
        {
            return Results.Ok(await kida.ReplaceRecoveryCodesAsync(token, request, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> BeginSamlAsync(
        [FromBody] KidaAuthEdgeSamlStartRequest request,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await edge.BeginSamlAuthenticationAsync(
                request.ProviderCode,
                request.ReturnUri,
                request.State,
                request.CodeChallenge,
                cancellationToken).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> CompleteSamlAsync(
        HttpContext context,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        SetNoStore(context);
        if (!context.Request.HasFormContentType) return InvalidRequest();
        try
        {
            var form = await context.Request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
            var handoff = await edge.CompleteSamlAuthenticationAsync(
                form["SAMLResponse"].ToString(),
                form["RelayState"].ToString(),
                cancellationToken).ConfigureAwait(false);
            var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(18));
            var destinationUri = new Uri(handoff.ReturnUri);
            context.Response.Headers.ContentSecurityPolicy =
                $"default-src 'none'; script-src 'nonce-{nonce}'; form-action {destinationUri.GetLeftPart(UriPartial.Authority)}; base-uri 'none'; frame-ancestors 'none'";
            var destination = WebUtility.HtmlEncode(handoff.ReturnUri);
            var code = WebUtility.HtmlEncode(handoff.Code);
            var state = WebUtility.HtmlEncode(handoff.State);
            return Results.Content($"""
                <!doctype html><html><head><meta charset="utf-8"><title>Completing sign-in</title></head>
                <body><form id="handoff" method="post" action="{destination}">
                <input type="hidden" name="kida_code" value="{code}">
                <input type="hidden" name="state" value="{state}"></form>
                <script nonce="{nonce}">document.getElementById('handoff').submit();</script></body></html>
                """, "text/html; charset=utf-8");
        }
        catch (Exception exception)
        {
            return Failure(exception);
        }
    }

    private static async Task<IResult> GetMfaEnrollmentAsync(
        string? ticket,
        HttpContext context,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        SetBrowserHeaders(context);
        try
        {
            var details = await edge.InspectTotpEnrollmentAsync(ticket ?? string.Empty, cancellationToken)
                .ConfigureAwait(false);
            var content = $"""
                <div class="qr">{QrCodeBuilder.CreateSvg(details.OtpauthUri)}</div>
                <form method="post">
                  <input type="hidden" name="ticket" value="{WebUtility.HtmlEncode(ticket)}">
                  <label>Authenticator code<input name="code" inputmode="numeric" autocomplete="one-time-code" pattern="[0-9][0-9][0-9][0-9][0-9][0-9]" maxlength="6" required autofocus></label>
                  <button type="submit">Verify and activate</button>
                </form>
                <p class="meta">{details.AttemptsRemaining} verification attempts remain. This setup expires {WebUtility.HtmlEncode(details.ExpiresAt.ToString("u"))}.</p>
                """;
            return MfaPage(
                "Set up your authenticator",
                "Scan this QR code with Microsoft Authenticator or another standards-compatible TOTP app, then enter the six-digit code.",
                content);
        }
        catch (Exception)
        {
            return MfaPage(
                "Authenticator setup unavailable",
                "This setup link is invalid, expired, or has already been used.",
                string.Empty,
                StatusCodes.Status400BadRequest);
        }
    }

    private static async Task<IResult> ConfirmMfaEnrollmentAsync(
        HttpContext context,
        [FromServices] IKidaAuthEdgeClient edge,
        CancellationToken cancellationToken)
    {
        SetBrowserHeaders(context);
        if (!context.Request.HasFormContentType)
        {
            return MfaPage(
                "Verification failed",
                "Submit the code from the authenticator setup page.",
                string.Empty,
                StatusCodes.Status400BadRequest);
        }

        try
        {
            var form = await context.Request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
            var completion = await edge.ConfirmTotpEnrollmentAsync(
                form["ticket"].ToString(),
                form["code"].ToString(),
                cancellationToken).ConfigureAwait(false);
            var codes = string.Join(
                string.Empty,
                completion.RecoveryCodes.Select(code => $"<li>{WebUtility.HtmlEncode(code)}</li>"));
            var continuation = completion.ReturnUri is null
                ? string.Empty
                : $"<a class=\"button\" href=\"{WebUtility.HtmlEncode(completion.ReturnUri)}\">Continue</a>";
            return MfaPage(
                "Authenticator activated",
                "Save these recovery codes now. Each code can be used once and Kida will not display them again.",
                $"<ul class=\"codes\">{codes}</ul>{continuation}");
        }
        catch (Exception)
        {
            return MfaPage(
                "Verification failed",
                "The code was incorrect, or the setup link has expired. Return to the setup link and try again.",
                string.Empty,
                StatusCodes.Status400BadRequest);
        }
    }

    private static IResult MfaPage(
        string title,
        string message,
        string content,
        int statusCode = StatusCodes.Status200OK) =>
        Results.Content(
            KidaAuthEdgeBrowserResources.MfaEnrollmentHtml
                .Replace("{{TITLE}}", WebUtility.HtmlEncode(title), StringComparison.Ordinal)
                .Replace("{{MESSAGE}}", WebUtility.HtmlEncode(message), StringComparison.Ordinal)
                .Replace("{{ENROLLMENT_CONTENT}}", content, StringComparison.Ordinal),
            "text/html; charset=utf-8",
            statusCode: statusCode);

    private static bool TryGetBearer(HttpContext context, out string token)
    {
        token = string.Empty;
        if (!AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization, out var authorization) ||
            !string.Equals(authorization.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(authorization.Parameter))
        {
            return false;
        }

        token = authorization.Parameter;
        return true;
    }

    private static IResult Failure(Exception exception)
    {
        if (exception is KidaRequestException requestException)
        {
            return Results.Problem(
                statusCode: (int)(requestException.StatusCode ?? HttpStatusCode.BadGateway),
                title: "Kida rejected the authentication request.",
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = requestException.ErrorCode ?? "kida.request_rejected"
                });
        }

        return Results.Problem(
            statusCode: StatusCodes.Status503ServiceUnavailable,
            title: "The internal Kida service is unavailable.",
            extensions: new Dictionary<string, object?>
            {
                ["code"] = "kida.service_unavailable"
            });
    }

    private static IResult InvalidRequest() => Results.Problem(
        statusCode: StatusCodes.Status400BadRequest,
        title: "The authentication request is invalid.",
        extensions: new Dictionary<string, object?>
        {
            ["code"] = "identity.invalid_request"
        });

    private static void SetNoStore(HttpContext context)
    {
        context.Response.Headers.CacheControl = "no-store";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.XContentTypeOptions = "nosniff";
        context.Response.Headers.XFrameOptions = "DENY";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
    }

    private static void SetBrowserHeaders(HttpContext context)
    {
        SetNoStore(context);
        context.Response.Headers.ContentSecurityPolicy =
            "default-src 'none'; style-src 'self'; script-src 'self'; connect-src 'self'; img-src 'self' data:; form-action 'self'; base-uri 'none'; frame-ancestors 'none'";
    }

    private static string NormalizePrefix(string value) => value.Trim().TrimEnd('/');
}
