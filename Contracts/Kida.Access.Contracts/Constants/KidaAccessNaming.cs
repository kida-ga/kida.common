using System.Text;

namespace Kida.Constants;
public static class KidaAccessNaming
{
    public static bool TryNormalizeCode(string? value, int maximumLength, out string normalized)
    {
        normalized = value?.Trim().Normalize().ToLowerInvariant() ?? string.Empty;
        return normalized.Length is >= 1 && normalized.Length <= maximumLength && char.IsAsciiLetterOrDigit(normalized[0]) && char.IsAsciiLetterOrDigit(normalized[^1]) && normalized.All(character => char.IsAsciiLetterOrDigit(character) || character is '.' or '_' or '-');
    }

    public static bool TryNormalizeDisplayName(string? value, out string normalized, int maximumLength = 250)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var source = value.Normalize();
        var result = new StringBuilder(source.Length);
        var pendingSpace = false;
        foreach (var character in source)
        {
            if (char.IsWhiteSpace(character))
            {
                pendingSpace = result.Length > 0;
                continue;
            }

            if (char.IsControl(character))
            {
                return false;
            }

            if (pendingSpace)
            {
                result.Append(' ');
                pendingSpace = false;
            }

            result.Append(character);
        }

        normalized = result.ToString();
        return normalized.Length is >= 1 && normalized.Length <= maximumLength;
    }

    public static bool TryNormalizeResource(string? value, out string normalized)
    {
        normalized = value?.Trim().Normalize() ?? string.Empty;
        return normalized.Length is >= 1 and <= 200 &&
               normalized.All(character => !char.IsWhiteSpace(character) && !char.IsControl(character));
    }
}
