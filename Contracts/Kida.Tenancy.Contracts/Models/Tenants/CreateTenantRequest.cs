namespace Kida.Models;

public sealed record CreateTenantRequest(
    string Code,
    string DisplayName,
    string Type = "organization");
