namespace Kida.Utils;

internal sealed class EmptyAttributes : Dictionary<string, string>
{
    internal static EmptyAttributes Instance { get; } = new();
}
