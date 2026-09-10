using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
namespace Kida.Service.Client;
public interface IKidaClient
{
    ValueTask<AuthenticationResult> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default);
    ValueTask<AuthenticationResult> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default);
    ValueTask RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    ValueTask<ProvisionedUser> ProvisionUserAsync(ProvisionLocalUserRequest request, CancellationToken cancellationToken = default);
    ValueTask<IdentityInvitationReceipt> InviteUserAsync(InviteIdentityRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<IdentityInvitationReceipt>(new NotSupportedException());
    ValueTask<ProvisionedUser> BootstrapUserAsync(BootstrapIdentityRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<ProvisionedUser>(new NotSupportedException());
    ValueTask<IdentityInvitationReceipt> MigrateLegacyUserAsync(MigrateLegacyIdentityRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<IdentityInvitationReceipt>(new NotSupportedException());
    ValueTask<VerificationGrantReceipt> VerifyInvitationAsync(VerifyIdentityInvitationRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<VerificationGrantReceipt>(new NotSupportedException());
    ValueTask<UserIdentity> CompleteEnrollmentAsync(CompleteIdentityEnrollmentRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<UserIdentity>(new NotSupportedException());
    ValueTask<FederatedIdentityResult> RedeemFederationHandoffAsync(RedeemFederationHandoffRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<FederatedIdentityResult>(new NotSupportedException());
    ValueTask<UserIdentity?> ResolveUserAsync(string username, CancellationToken cancellationToken = default);
    ValueTask<UserLoginAttemptPage> ListUserLoginAttemptsAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default) => ValueTask.FromException<UserLoginAttemptPage>(new NotSupportedException());
    ValueTask SetUserStatusAsync(Guid userId, SetUserStatusRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException(new NotSupportedException());
    ValueTask ReleaseUserLoginProtectionAsync(Guid userId, CancellationToken cancellationToken = default) => ValueTask.FromException(new NotSupportedException());
    ValueTask ResetUserPasswordAsync(Guid userId, ResetUserPasswordRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException(new NotSupportedException());
    ValueTask ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    ValueTask<PasswordResetInitiationResult> BeginPasswordResetAsync(BeginPasswordResetRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<PasswordResetInitiationResult>(new NotSupportedException());
    ValueTask<OAuthScopeRegistrationResult> RegisterScopesAsync(RegisterOAuthScopesRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> RegisterModuleAsync(RegisterModuleRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<AccessModuleInfo>> ListModulesAsync(CancellationToken cancellationToken = default) => ValueTask.FromException<IReadOnlyCollection<AccessModuleInfo>>(new NotSupportedException());
    ValueTask<AccessDecision> EvaluateAccessAsync(AccessDecisionRequest request, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicyVersionInfo> GetAccessPolicyVersionAsync(Guid tenantId, string module, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicySnapshot> GetAccessPolicySnapshotAsync(Guid tenantId, string module, CancellationToken cancellationToken = default);
    ValueTask<AccessPolicyDefinitionSnapshot> GetAccessPolicyDefinitionAsync(Guid tenantId, string module, CancellationToken cancellationToken = default) => ValueTask.FromException<AccessPolicyDefinitionSnapshot>(new NotSupportedException());
    ValueTask<SubjectEntitlementRevisionInfo> GetSubjectEntitlementRevisionAsync(Guid tenantId, SubjectEntitlementQuery request, CancellationToken cancellationToken = default) => ValueTask.FromException<SubjectEntitlementRevisionInfo>(new NotSupportedException());
    ValueTask<SubjectEntitlementSnapshot> GetSubjectEntitlementSnapshotAsync(Guid tenantId, SubjectEntitlementQuery request, CancellationToken cancellationToken = default) => ValueTask.FromException<SubjectEntitlementSnapshot>(new NotSupportedException());
    ValueTask<ClientResourceGrantSnapshot> GetClientResourceGrantSnapshotAsync(Guid clientId, string resource, CancellationToken cancellationToken = default) => ValueTask.FromException<ClientResourceGrantSnapshot>(new NotSupportedException());
    async ValueTask<ClientResourceGrantRevision> GetClientResourceGrantRevisionAsync(Guid clientId, string resource, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetClientResourceGrantSnapshotAsync(clientId, resource, cancellationToken).ConfigureAwait(false);
        return new(snapshot.ClientId, snapshot.Audience, snapshot.Status, snapshot.Version, snapshot.RevisionHash, snapshot.ModifiedAt);
    }
    ValueTask<IReadOnlyCollection<AccessRoleInfo>> ListRolesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    ValueTask GrantRoleActionAsync(GrantRoleActionRequest request, CancellationToken cancellationToken = default);
    ValueTask ApplyRoleActionsAsync(ApplyRoleActionsRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException(new NotSupportedException());
    ValueTask EnsureRoleActionAsync(EnsureRoleActionRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<RoleAssignmentInfo>> ListAssignmentsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    ValueTask<AccessResult> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    ValueTask RevokeRoleAssignmentAsync(Guid tenantId, Guid assignmentId, CancellationToken cancellationToken = default) => ValueTask.FromException(new NotSupportedException());
    ValueTask<TenantPage> SearchTenantsAsync(TenantSearchRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<TenantPage>(new NotSupportedException());
    ValueTask<TenantSummary> CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<TenantSummary>(new NotSupportedException());
    ValueTask<TenantSummary> UpdateTenantAsync(Guid tenantId, UpdateTenantRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<TenantSummary>(new NotSupportedException());
    ValueTask<UserIdentity> GetCurrentUserAsync(string userAccessToken, CancellationToken cancellationToken = default);
    ValueTask<UserProfile> GetProfileAsync(string userAccessToken, CancellationToken cancellationToken = default);
    ValueTask<UserProfile> UpdateProfileAsync(string userAccessToken, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<MfaMethodInfo>> ListMfaMethodsAsync(string userAccessToken, CancellationToken cancellationToken = default) => ValueTask.FromException<IReadOnlyCollection<MfaMethodInfo>>(new NotSupportedException());
    ValueTask<TotpEnrollmentReceipt> BeginTotpEnrollmentAsync(string userAccessToken, BeginTotpEnrollmentRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<TotpEnrollmentReceipt>(new NotSupportedException());
    ValueTask<RecoveryCodesReceipt> ReplaceRecoveryCodesAsync(string userAccessToken, ReplaceRecoveryCodesRequest request, CancellationToken cancellationToken = default) => ValueTask.FromException<RecoveryCodesReceipt>(new NotSupportedException());
}
