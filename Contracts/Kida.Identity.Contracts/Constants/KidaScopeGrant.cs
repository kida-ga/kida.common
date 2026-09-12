namespace Kida.Constants;

/// <summary>
/// Defines persisted OAuth client scope-grant markers. A wildcard always belongs
/// to one exact client-resource grant; it is never a cross-audience authority.
/// </summary>
public static class KidaScopeGrant
{
    public const string All = "*";
    public const string AllMode = "all";

    public static bool IsAll(string? value) =>
        string.Equals(value?.Trim(), All, StringComparison.Ordinal);

    public static bool IsAll(IEnumerable<string>? scopes) =>
        scopes?.Count() == 1 && scopes.Any(IsAll);
}
