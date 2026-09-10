namespace Kida.Models;

/// <summary>One ordered level in a product-owned resource path.</summary>
public sealed record AccessScopeRef(string Type, Guid? Id);
