namespace Kida.Models;

public sealed record TenantAdministrationSnapshot(
    TenantSummary Tenant,
    IReadOnlyCollection<TenantDomainInfo> Domains,
    IReadOnlyCollection<TenantMembershipInfo> Memberships,
    IReadOnlyCollection<TenantInvitationInfo> Invitations,
    IReadOnlyCollection<TenantSettingInfo> Settings,
    IReadOnlyCollection<TenantDeploymentInfo> Deployments);
