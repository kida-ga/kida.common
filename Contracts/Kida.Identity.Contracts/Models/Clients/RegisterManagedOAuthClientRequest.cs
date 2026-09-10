namespace Kida.Models;

/// <summary>
/// Registers a machine-to-machine integration beneath the single product audience
/// assigned to the authenticated management client. Audience and owner tenant are
/// deliberately absent and are always derived by Kida.
/// </summary>
public sealed record RegisterManagedOAuthClientRequest(
    string ClientIdentifier,
    string DisplayName,
    IReadOnlyCollection<string> AllowedScopes,
    uint AccessTokenSeconds = 300);
