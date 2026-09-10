using System.Text;
using Haley.Utils;

namespace Kida.Utils;

internal static class KidaAuthEdgeBrowserResources
{
    internal static readonly string PasswordChangeHtml = Read("password-change.html");
    internal static readonly string PasswordChangeCss = Read("password-change.css");
    internal static readonly string PasswordChangeJavaScript = Read("password-change.js");
    internal static readonly string PasswordResetHtml = Read("password-reset.html");
    internal static readonly string PasswordResetCss = Read("password-reset.css");
    internal static readonly string PasswordResetJavaScript = Read("password-reset.js");
    internal static readonly string MfaEnrollmentHtml = Read("mfa-enroll.html");
    internal static readonly string MfaEnrollmentCss = Read("mfa-enroll.css");

    private static string Read(string fileName)
    {
        var resourceName = $"Kida.AuthEdge.Browser.{fileName}";
        var content = ResourceUtils.GetEmbeddedResource(
            resourceName,
            typeof(KidaAuthEdgeBrowserResources).Assembly)
            ?? throw new InvalidOperationException($"Embedded Auth Edge resource '{resourceName}' was not found.");
        return Encoding.UTF8.GetString(content);
    }
}
