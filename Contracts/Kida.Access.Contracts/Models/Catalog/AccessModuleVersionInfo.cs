using System.Text;

namespace Kida.Models;
public sealed record AccessModuleVersionInfo(Guid VersionId, string Version, DateTimeOffset FirstSeenAt, DateTimeOffset LastSeenAt);
