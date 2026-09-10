using System.Text;

namespace Kida.Models;
/// <summary>Associates one module-owned client scope with one module-owned action.</summary>
public sealed record RegisterScopeActionRequest(string Scope, string Action);
