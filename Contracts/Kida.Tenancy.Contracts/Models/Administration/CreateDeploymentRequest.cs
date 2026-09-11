using System.Text.Json;

namespace Kida.Models;

public sealed record CreateDeploymentRequest(string Code, string Profile, string? Region, string? ResidencyCode, string? BaseUri, JsonElement? Configuration);
