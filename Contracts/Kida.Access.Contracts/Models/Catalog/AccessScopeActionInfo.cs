using System.Text;

namespace Kida.Models;
public sealed record AccessScopeActionInfo(Guid MappingId, string Scope, Guid ActionId, string Action, string Status, IReadOnlyCollection<string> RegisteredVersions);
