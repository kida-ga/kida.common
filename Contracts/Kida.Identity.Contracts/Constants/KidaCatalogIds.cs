using Haley.Enums;
using Haley.Utils;

namespace Kida.Constants;

/// <summary>
/// Stable public identities shared by the Identity and Access projections of a
/// module catalog. Codes are normalized before they reach this boundary.
/// </summary>
public static class KidaCatalogIds
{
    private const string ModuleNamespace = "kida.security.module:v1:";
    private const string VersionNamespace = "kida.security.module-version:v1:";
    private const string ScopeNamespace = "kida.security.scope:v1:";
    private const string ActionNamespace = "kida.security.action:v1:";
    private const string ScopeActionNamespace = "kida.security.scope-action:v1:";

    public static Guid Module(string moduleCode) =>
        $"{ModuleNamespace}{moduleCode}".CreateGUID(HashMethod.Sha256);

    public static Guid ModuleVersion(string moduleCode, string version) =>
        $"{VersionNamespace}{moduleCode}:{version}".CreateGUID(HashMethod.Sha256);

    public static Guid Scope(string moduleCode, string scopeCode) =>
        $"{ScopeNamespace}{moduleCode}:{scopeCode}".CreateGUID(HashMethod.Sha256);

    public static Guid Action(string moduleCode, string actionCode) =>
        $"{ActionNamespace}{moduleCode}:{actionCode}".CreateGUID(HashMethod.Sha256);

    public static Guid ScopeAction(string moduleCode, string scopeCode, string actionCode) =>
        $"{ScopeActionNamespace}{moduleCode}:{scopeCode}:{actionCode}".CreateGUID(HashMethod.Sha256);
}
