using Haley.Utils;
using Kida.Tool.Models;

namespace Kida.Tool.Services;

internal static class ApplicationInfoReader
{
    internal static ApplicationInfoContext Read(EnvelopeOptions options)
    {
        var path = Path.GetFullPath(options.AppInfoPath);
        var info = DeploymentUtils.ReadApplicationInfo(path);
        var baseDirectory = Path.GetFullPath(string.IsNullOrWhiteSpace(options.BaseDirectory)
            ? Path.GetDirectoryName(path)!
            : options.BaseDirectory);
        var configuration = ResourceUtils.GenerateConfigurationRoot(
            ["appsettings.json", Path.Combine("Config", "appsettings.json")],
            baseDirectory);
        var deploymentInfoLocation = configuration[DeploymentUtils.DeploymentInfoLocationConfigurationKey];

        return new ApplicationInfoContext
        {
            Info = info,
            AppInfoPath = path,
            BaseDirectory = baseDirectory,
            DeploymentInfoLocation = string.IsNullOrWhiteSpace(deploymentInfoLocation)
                ? null
                : deploymentInfoLocation.Trim()
        };
    }
}
