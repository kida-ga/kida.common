using Haley.Abstractions;
using Kida.Models;

namespace Kida.Abstractions;

public interface ITenancyAdministrationService
{
    ValueTask<IFeedback<TenantSummary>> CreateAsync(
        CreateTenantRequest request,
        CancellationToken cancellationToken = default);

    ValueTask<IFeedback<TenantSummary>> UpdateAsync(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default);

    ValueTask<IFeedback> PermanentlyDeleteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    ValueTask<TenantAdministrationSnapshot?> InspectAsync(Guid tenantId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<TenantDomainInfo>> AddDomainAsync(AddTenantDomainRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RemoveDomainAsync(Guid tenantId, Guid domainId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<TenantMembershipInfo>> SetMembershipAsync(SetTenantMembershipRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RemoveMembershipAsync(Guid tenantId, Guid membershipId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<TenantInvitationReceipt>> CreateInvitationAsync(CreateTenantInvitationRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RevokeInvitationAsync(Guid tenantId, Guid invitationId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<TenantSettingInfo>> SetSettingAsync(SetTenantSettingRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> RemoveSettingAsync(Guid tenantId, string key, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<DeploymentInfo>> ListDeploymentsAsync(CancellationToken cancellationToken = default);
    ValueTask<IFeedback<DeploymentInfo>> CreateDeploymentAsync(CreateDeploymentRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<TenantDeploymentInfo>> AssignDeploymentAsync(AssignTenantDeploymentRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> UnassignDeploymentAsync(Guid tenantId, Guid deploymentId, string familyCode, CancellationToken cancellationToken = default);
}
