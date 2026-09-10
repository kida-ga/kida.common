using System.Text;

namespace Kida.Models;
public sealed record AccessDecisionRequest(
    string Resource,
    Guid TenantId,
    Guid SubjectId,
    string SubjectType,
    string Module,
    string Action,
    string ScopeType = "tenant",
    Guid? ScopeId = null,
    IReadOnlyDictionary<string, string>? Attributes = null,
    IReadOnlyCollection<AccessSubjectRef>? Subjects = null,
    IReadOnlyCollection<AccessScopeRef>? ScopePath = null,
    Guid? AuthorizedClientId = null);
