namespace Kida.Models;

public sealed record KidaAuthEdgePasswordResetCompleteRequest(
    Guid GrantId,
    string NewPassword,
    string? ReturnUri = null,
    string? State = null);
