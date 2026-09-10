using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
namespace Kida.Service.Client;
public interface IKidaAccessPolicyCache
{
    ValueTask<AccessPolicySnapshot> GetSnapshotAsync(Guid tenantId, string module, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicyDefinitionSnapshot> GetDefinitionAsync(Guid tenantId, string module, CancellationToken cancellationToken = default);
    ValueTask<SubjectEntitlementSnapshot> GetSubjectSnapshotAsync(Guid tenantId, string module, IReadOnlyCollection<AccessSubjectRef> subjects, IReadOnlyCollection<AccessScopeRef> scopePath, CancellationToken cancellationToken = default);
    ValueTask<AccessDecision> EvaluateAsync(AccessDecisionRequest request, CancellationToken cancellationToken = default);
    void Invalidate(Guid tenantId, string module);
}
