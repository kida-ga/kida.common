using System.Text.Json;

namespace Kida.Models;

public sealed record AppEndpointDeclaration(
    string Name,
    string Type,
    string UriTemplate,
    string AuthMode,
    string? HealthUri,
    JsonElement? Configuration);
