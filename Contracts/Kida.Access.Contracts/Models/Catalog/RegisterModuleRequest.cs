using System.Text;

namespace Kida.Models;
/// <summary>
/// Publishes a code-owned Access catalog. Kida derives module and action UUIDs
/// from the normalized codes; callers must not create or persist those UUIDs.
/// </summary>
public sealed record RegisterModuleRequest(string Resource, string Code, string DisplayName, string OwnerFamily, string Version, IReadOnlyCollection<RegisterActionRequest> Actions, IReadOnlyCollection<RegisterScopeActionRequest> ScopeActions);
