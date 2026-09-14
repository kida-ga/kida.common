using System.Text;

namespace Kida.Abstractions;
public interface IAccessQueryService
{
    ValueTask<IReadOnlyCollection<AccessModuleInfo>> ListModulesAsync(CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<AccessRoleInfo>> ListRolesAsync(Guid tenantId, string resource, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<AccessRoleActionInfo>> ListRoleActionsAsync(Guid tenantId, Guid roleId, string resource, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<RoleAssignmentInfo>> ListAssignmentsAsync(Guid tenantId, string resource, CancellationToken cancellationToken = default);
    ValueTask<RoleAssignmentPage> SearchSubjectAssignmentsAsync(SubjectAssignmentSearchRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicyVersionInfo?> GetPolicyVersionAsync(Guid tenantId, string resource, string module, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicySnapshot?> GetPolicySnapshotAsync(Guid tenantId, string resource, string module, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicyDefinitionSnapshot?> GetPolicyDefinitionAsync(Guid tenantId, string resource, string module, CancellationToken cancellationToken = default);
    ValueTask<SubjectEntitlementRevisionInfo?> GetSubjectRevisionAsync(Guid tenantId, SubjectEntitlementQuery request, CancellationToken cancellationToken = default);
    ValueTask<SubjectEntitlementSnapshot?> GetSubjectSnapshotAsync(Guid tenantId, SubjectEntitlementQuery request, CancellationToken cancellationToken = default);
}
