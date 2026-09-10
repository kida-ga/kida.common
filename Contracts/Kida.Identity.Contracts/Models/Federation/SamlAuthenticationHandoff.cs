using Haley.Abstractions;

namespace Kida.Models;
/// <summary>
/// A short-lived, single-use authorization-code-style handoff. The browser
/// receives this code in a form POST; it never receives a Kida token.
/// </summary>
public sealed record SamlAuthenticationHandoff(string ReturnUri, string Code, DateTimeOffset ExpiresAt, string State = "");
