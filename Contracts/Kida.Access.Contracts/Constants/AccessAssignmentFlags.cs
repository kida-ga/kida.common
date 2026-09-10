namespace Kida.Constants;

/// <summary>Bit flags controlling inheritance and protected access boundaries.</summary>
public static class AccessAssignmentFlags
{
    public const uint None = 0;
    public const uint AppliesToDescendants = 1u << 0;
    public const uint Protected = 1u << 1;
    public const uint KnownMask = AppliesToDescendants | Protected;

    public static bool Has(uint value, uint flag) => (value & flag) == flag;
}
