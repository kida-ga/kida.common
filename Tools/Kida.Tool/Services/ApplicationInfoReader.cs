using Haley.Utils;
using Kida.Tool.Models;

namespace Kida.Tool.Services;

internal static class ApplicationInfoReader
{
    internal static ApplicationInfoContext Read(EnvelopeOptions options)
    {
        var path = Path.GetFullPath(options.AppInfoPath);
        var info = DeploymentUtils.ReadApplicationInfo(path);

        return new ApplicationInfoContext
        {
            Info = info,
            AppInfoPath = path,
            BaseDirectory = Path.GetFullPath(string.IsNullOrWhiteSpace(options.BaseDirectory)
                ? Path.GetDirectoryName(path)!
                : options.BaseDirectory)
        };
    }
}
