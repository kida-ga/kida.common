using System.Text;

namespace Kida.Constants;
public static class KidaAccessScopes
{
    public const string Administer = "access.administer";
    public const string Decide = "access.decide";
    public const string ModuleRegister = "access.module.register";
    public const string ProtectedManage = "access.protected.manage";
    public static bool TryNormalizeModuleCode(string? value, out string normalized) => KidaAccessNaming.TryNormalizeCode(value, 150, out normalized);
}
