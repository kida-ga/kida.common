namespace Kida.Models;

/// <summary>
/// Requests one client token. Omitting Resource selects Kida's stable internal
/// service audience. A product-hosted authentication edge may bind Resource to
/// its configured product audience; callers never select a scope subset.
/// </summary>
public sealed record ClientTokenRequest(
    string ClientIdentifier,
    string ClientSecret,
    string? Resource = null);
