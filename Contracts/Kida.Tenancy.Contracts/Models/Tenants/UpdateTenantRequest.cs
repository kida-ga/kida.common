namespace Kida.Models;

public sealed record UpdateTenantRequest(
    string DisplayName,
    string Status);
