using System.Text.Json;

namespace Kida.Models;

public sealed record AppInstallationGrant(string Type, string Code, JsonElement? Boundary);
