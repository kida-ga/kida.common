using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Constants;
public static class KidaResourceAudience
{
    public static bool TryNormalize(string? value, out string audience)
    {
        audience = value?.Trim().Normalize() ?? string.Empty;
        return audience.Length is >= 1 and <= 200 && audience.All(character => !char.IsWhiteSpace(character) && !char.IsControl(character));
    }
}
