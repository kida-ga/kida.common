using System.Text;

namespace Kida.Abstractions;
public interface IAccessAdministrationService
{
    ValueTask<AccessResult> PlanModuleAsync(PlanAccessModuleRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> DeleteUnusedPlannedModuleAsync(string moduleCode, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> RegisterModuleAsync(RegisterModuleRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> GrantRoleActionAsync(GrantRoleActionRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> ApplyRoleActionsAsync(ApplyRoleActionsRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> EnsureRoleActionAsync(EnsureRoleActionRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> UpdateRoleAssignmentAsync(UpdateRoleAssignmentRequest request, bool allowProtected = false, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> RevokeRoleAssignmentAsync(RevokeRoleAssignmentRequest request, bool allowProtected = false, CancellationToken cancellationToken = default);
}
