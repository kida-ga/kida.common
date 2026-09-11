using System.Text.Json;

namespace Kida.Models;

public sealed record AppContributionDeclaration(
    string Type,
    string Key,
    string? DisplayName,
    string? Target,
    int Order,
    JsonElement Configuration);
