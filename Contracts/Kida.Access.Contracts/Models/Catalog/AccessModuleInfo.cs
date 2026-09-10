using System.Text;

namespace Kida.Models;
public sealed record AccessModuleInfo(Guid ModuleId, string Code, string DisplayName, string OwnerFamily, string Status, IReadOnlyCollection<AccessModuleVersionInfo> Versions, IReadOnlyCollection<AccessActionInfo> Actions, IReadOnlyCollection<AccessScopeActionInfo> ScopeActions);
