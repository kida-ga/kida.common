namespace Kida.Models;

/// <summary>Maps one client-facing module scope to one fine-grained action.</summary>
public sealed record AccessScopeActionGrantInfo(string Scope, string Action);
