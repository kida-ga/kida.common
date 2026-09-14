using Haley.Models;

namespace Kida.Tool.Models;

internal sealed class ApplicationInfoContext
{
    internal required DeploymentApplicationInfo Info { get; init; }
    internal required string AppInfoPath { get; init; }
    internal required string BaseDirectory { get; init; }
}
