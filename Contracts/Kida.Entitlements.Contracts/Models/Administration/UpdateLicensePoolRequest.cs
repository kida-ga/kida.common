namespace Kida.Models;

public sealed record UpdateLicensePoolRequest(
    int Quantity,
    string Status,
    string? DisplayName);
