using System.Net;
using Haley.Abstractions;
using Haley.Enums;
using Haley.Models;
using Haley.Utils;
using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaClient(
    KidaClientTransport transport,
    IOptions<KidaClientOptions> options,
    IKidaClientTokenProvider clientTokenProvider) : IKidaClient, IKidaAuthEdgeClient
{
    private readonly KidaClientOptions _options = options.Value;

    public async ValueTask<AuthenticationResult> AuthenticateAsync(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default)
    {
        var boundRequest = request with
        {
            ClientId = _options.ClientId,
            Resource = _options.UserAudience
        };
        return await SendWithClientTokenAsync<AuthenticationResult>(
            () => AddIdentityHeaders(
                CreateJsonRequest("kida/identity/sessions", boundRequest, cancellationToken),
                username: boundRequest.Username),
            Method.POST,
            KidaIdentityScopes.Authenticate,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<AuthenticationResult> RefreshSessionAsync(
        string refreshToken,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AuthenticationResult>(
            () => CreateJsonRequest(
                "kida/identity/sessions/refresh",
                new RefreshSessionRequest(refreshToken, _options.ClientId),
                cancellationToken),
            Method.POST,
            KidaIdentityScopes.Authenticate,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<ProvisionedUser> ProvisionUserAsync(
        ProvisionLocalUserRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<ProvisionedUser>(
            () => CreateJsonRequest(
                "kida/identity/users/provision",
                request with { ClientId = _options.ClientId, Resource = _options.UserAudience },
                cancellationToken),
            Method.POST,
            KidaIdentityScopes.UsersCreate,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<IdentityInvitationReceipt> InviteUserAsync(InviteIdentityRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IdentityInvitationReceipt>(
            () => CreateJsonRequest("kida/identity/users/invitations", request with { ClientId = _options.ClientId, Resource = _options.UserAudience }, cancellationToken),
            Method.POST, KidaIdentityScopes.UsersInvite, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ProvisionedUser> BootstrapUserAsync(BootstrapIdentityRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<ProvisionedUser>(
            () => CreateJsonRequest("kida/identity/users/bootstrap", request with { ClientId = _options.ClientId, Resource = _options.UserAudience }, cancellationToken),
            Method.POST, KidaIdentityScopes.UsersBootstrap, cancellationToken).ConfigureAwait(false);

    public async ValueTask<IdentityInvitationReceipt> MigrateLegacyUserAsync(MigrateLegacyIdentityRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IdentityInvitationReceipt>(
            () => CreateJsonRequest("kida/identity/users/migrations", request with { ClientId = _options.ClientId, Resource = _options.UserAudience }, cancellationToken),
            Method.POST, KidaIdentityScopes.UsersMigrate, cancellationToken).ConfigureAwait(false);

    public async ValueTask<VerificationGrantReceipt> VerifyInvitationAsync(VerifyIdentityInvitationRequest request, CancellationToken cancellationToken = default) =>
        await SendAnonymousAsync<VerificationGrantReceipt>("kida/identity/verifications/complete", request, cancellationToken).ConfigureAwait(false);

    public async ValueTask<UserIdentity> CompleteEnrollmentAsync(CompleteIdentityEnrollmentRequest request, CancellationToken cancellationToken = default) =>
        await SendAnonymousAsync<UserIdentity>("kida/identity/enrollments/password", request with { ClientId = _options.ClientId }, cancellationToken).ConfigureAwait(false);

    public async ValueTask<FederatedIdentityResult> RedeemFederationHandoffAsync(RedeemFederationHandoffRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<FederatedIdentityResult>(
            () => CreateJsonRequest("kida/identity/federation/handoffs", request with { ClientId = _options.ClientId }, cancellationToken),
            Method.POST, KidaIdentityScopes.FederationExchange, cancellationToken).ConfigureAwait(false);

    public async ValueTask<UserIdentity?> ResolveUserAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        try
        {
            return await SendWithClientTokenAsync<UserIdentity>(
                () => AddIdentityHeaders(
                    CreateRequest("kida/identity/users/resolve", cancellationToken)
                        .WithQuery(new QueryParam("username", username.Trim())),
                    username: username),
                Method.GET,
                KidaIdentityScopes.UsersResolve,
                cancellationToken).ConfigureAwait(false);
        }
        catch (KidaRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async ValueTask<UserLoginAttemptPage> ListUserLoginAttemptsAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<UserLoginAttemptPage>(
            () => CreateRequest($"kida/identity/users/{userId:D}/login-attempts", cancellationToken)
                .WithQuery(new QueryParam("page", Math.Max(1, page)))
                .WithQuery(new QueryParam("pageSize", Math.Clamp(pageSize, 1, 50))),
            Method.GET,
            KidaIdentityScopes.UsersManage,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask SetUserStatusAsync(
        Guid userId,
        SetUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) throw new ArgumentException("A user identifier is required.", nameof(userId));
        await SendWithClientTokenWithoutBodyAsync(
            () => CreateJsonRequest($"kida/identity/users/{userId:D}/status", request, cancellationToken),
            Method.PUT,
            KidaIdentityScopes.UsersManage,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ReleaseUserLoginProtectionAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) throw new ArgumentException("A user identifier is required.", nameof(userId));
        await SendWithClientTokenWithoutBodyAsync(
            () => CreateRequest($"kida/identity/users/{userId:D}/login-protection/release", cancellationToken),
            Method.POST,
            KidaIdentityScopes.UsersManage,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ResetUserPasswordAsync(
        Guid userId,
        ResetUserPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty || request.UserId != userId)
            throw new ArgumentException("The route and request user identifiers must match.", nameof(userId));

        await SendWithClientTokenWithoutBodyAsync(
            () => CreateJsonRequest(
                $"kida/identity/users/{userId:D}/password",
                new
                {
                    request.NewPassword,
                    request.RequirePasswordChange,
                    request.ReasonCode
                },
                cancellationToken),
            Method.PUT,
            KidaIdentityScopes.UsersManage,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ChangePasswordAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await AddIdentityHeaders(
                CreateJsonRequest(
                    "kida/identity/password/change",
                    request,
                    cancellationToken),
                username: request.Username,
                includeClientIdentifier: false)
            .DoNotAuthenticate()
            .PostAsync()
            .ConfigureAwait(false);
        await KidaResponseReader.EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async ValueTask<PasswordResetInitiationResult> BeginPasswordResetAsync(
        BeginPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        var boundRequest = request with
        {
            ClientId = _options.ClientId,
            Resource = string.IsNullOrWhiteSpace(request.Resource)
                ? _options.UserAudience
                : request.Resource
        };
        return await SendWithClientTokenAsync<PasswordResetInitiationResult>(
            () => AddIdentityHeaders(
                CreateJsonRequest("kida/identity/password/resets", boundRequest, cancellationToken),
                username: boundRequest.Destination),
            Method.POST,
            KidaIdentityScopes.PasswordResetRequest,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RevokeSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        if (sessionId == Guid.Empty) throw new ArgumentException("A session identifier is required.", nameof(sessionId));

        await SendWithClientTokenWithoutBodyAsync(
            () => CreateRequest($"kida/identity/client-sessions/{sessionId:D}", cancellationToken),
            Method.DELETE,
            KidaIdentityScopes.SessionsRevoke,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<OAuthScopeRegistrationResult> RegisterScopesAsync(
        RegisterOAuthScopesRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<OAuthScopeRegistrationResult>(
            () => CreateJsonRequest("kida/identity/scopes/registrations", request, cancellationToken),
            Method.PUT,
            Kida.Constants.KidaAccessScopes.ModuleRegister,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<AccessResult> RegisterModuleAsync(
        RegisterModuleRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AccessResult>(
            () => CreateJsonRequest(
                $"kida/access/modules/{Uri.EscapeDataString(request.Code)}",
                request,
                cancellationToken),
            Method.PUT,
            KidaAccessScopes.ModuleRegister,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<IReadOnlyCollection<AccessModuleInfo>> ListModulesAsync(
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IReadOnlyCollection<AccessModuleInfo>>(
            () => CreateRequest("kida/access/modules", cancellationToken),
            Method.GET,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<AccessDecision> EvaluateAccessAsync(
        AccessDecisionRequest request,
        CancellationToken cancellationToken = default)
    {
        var boundedRequest = string.IsNullOrWhiteSpace(request.Resource)
            ? request with { Resource = _options.UserAudience }
            : request;
        return await SendWithClientTokenAsync<AccessDecision>(
            () => CreateJsonRequest(
                "kida/access/decisions",
                boundedRequest,
                cancellationToken),
            Method.POST,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<AccessPolicyVersionInfo> GetAccessPolicyVersionAsync(
        Guid tenantId,
        string module,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AccessPolicyVersionInfo>(
            () => CreateRequest($"kida/access/policies/{tenantId:D}/version", cancellationToken)
                .WithQuery(new QueryParam("module", module))
                .WithQuery(new QueryParam("resource", _options.UserAudience)),
            Method.GET,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<AccessPolicySnapshot> GetAccessPolicySnapshotAsync(
        Guid tenantId,
        string module,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AccessPolicySnapshot>(
            () => CreateRequest($"kida/access/policies/{tenantId:D}/snapshot", cancellationToken)
                .WithQuery(new QueryParam("module", module))
                .WithQuery(new QueryParam("resource", _options.UserAudience)),
            Method.GET,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<AccessPolicyDefinitionSnapshot> GetAccessPolicyDefinitionAsync(
        Guid tenantId,
        string module,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AccessPolicyDefinitionSnapshot>(
            () => CreateRequest($"kida/access/policies/{tenantId:D}/definition", cancellationToken)
                .WithQuery(new QueryParam("module", module))
                .WithQuery(new QueryParam("resource", _options.UserAudience)),
            Method.GET,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<SubjectEntitlementRevisionInfo> GetSubjectEntitlementRevisionAsync(
        Guid tenantId,
        SubjectEntitlementQuery request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<SubjectEntitlementRevisionInfo>(
            () => CreateJsonRequest(
                $"kida/access/policies/{tenantId:D}/subjects/revision",
                request with { Resource = _options.UserAudience },
                cancellationToken),
            Method.POST,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<SubjectEntitlementSnapshot> GetSubjectEntitlementSnapshotAsync(
        Guid tenantId,
        SubjectEntitlementQuery request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<SubjectEntitlementSnapshot>(
            () => CreateJsonRequest(
                $"kida/access/policies/{tenantId:D}/subjects/snapshot",
                request with { Resource = _options.UserAudience },
                cancellationToken),
            Method.POST,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<ClientResourceGrantSnapshot> GetClientResourceGrantSnapshotAsync(
        Guid clientId,
        string resource,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<ClientResourceGrantSnapshot>(
            () => CreateRequest("kida/identity/client-resource-grant", cancellationToken)
                .WithQuery(new QueryParam("resource", resource))
                .WithQuery(new QueryParam("clientId", clientId.ToString("D"))),
            Method.GET,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<ClientResourceGrantRevision> GetClientResourceGrantRevisionAsync(
        Guid clientId,
        string resource,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<ClientResourceGrantRevision>(
            () => CreateRequest("kida/identity/client-resource-grant/revision", cancellationToken)
                .WithQuery(new QueryParam("resource", resource))
                .WithQuery(new QueryParam("clientId", clientId.ToString("D"))),
            Method.GET,
            KidaAccessScopes.Decide,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<IReadOnlyCollection<AccessRoleInfo>> ListRolesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IReadOnlyCollection<AccessRoleInfo>>(
            () => CreateRequest("kida/access/roles", cancellationToken)
                .WithQuery(new QueryParam("tenantId", tenantId.ToString("D")))
                .WithQuery(new QueryParam("resource", _options.UserAudience)),
            Method.GET,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<AccessResult> CreateRoleAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AccessResult>(
            () => CreateJsonRequest("kida/access/roles", request with { Resource = _options.UserAudience }, cancellationToken),
            Method.POST,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask GrantRoleActionAsync(
        GrantRoleActionRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(
            () => CreateJsonRequest(
                $"kida/access/roles/{request.RoleId:D}/actions",
                request with { Resource = _options.UserAudience },
                cancellationToken),
            Method.PUT,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask EnsureRoleActionAsync(
        EnsureRoleActionRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(
            () => CreateJsonRequest(
                $"kida/access/roles/{request.RoleId:D}/actions/ensure",
                request with { Resource = _options.UserAudience },
                cancellationToken),
            Method.PUT,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<IReadOnlyCollection<RoleAssignmentInfo>> ListAssignmentsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IReadOnlyCollection<RoleAssignmentInfo>>(
            () => CreateRequest("kida/access/assignments", cancellationToken)
                .WithQuery(new QueryParam("tenantId", tenantId.ToString("D")))
                .WithQuery(new QueryParam("resource", _options.UserAudience)),
            Method.GET,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<AccessResult> AssignRoleAsync(
        AssignRoleRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AccessResult>(
            () => CreateJsonRequest("kida/access/assignments", request with { Resource = _options.UserAudience }, cancellationToken),
            Method.POST,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask ApplyRoleActionsAsync(
        ApplyRoleActionsRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(
            () => CreateJsonRequest(
                $"kida/access/roles/{request.RoleId:D}/actions/batch",
                request with { Resource = _options.UserAudience },
                cancellationToken),
            Method.PUT,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask RevokeRoleAssignmentAsync(
        Guid tenantId,
        Guid assignmentId,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(
            () => CreateRequest($"kida/access/assignments/{assignmentId:D}", cancellationToken)
                .WithQuery(new QueryParam("tenantId", tenantId.ToString("D")))
                .WithQuery(new QueryParam("resource", _options.UserAudience)),
            Method.DELETE,
            KidaAccessScopes.Administer,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantPage> SearchTenantsAsync(
        TenantSearchRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantPage>(
            () => CreateRequest("kida/tenancy/tenants", cancellationToken)
                .WithQuery(new QueryParam("query", request.Query ?? string.Empty))
                .WithQuery(new QueryParam("status", request.Status ?? string.Empty))
                .WithQuery(new QueryParam("page", Math.Max(1, request.Page).ToString(System.Globalization.CultureInfo.InvariantCulture)))
                .WithQuery(new QueryParam("pageSize", Math.Clamp(request.PageSize, 1, 50).ToString(System.Globalization.CultureInfo.InvariantCulture))),
            Method.GET,
            KidaTenancyScopes.Read,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantSummary> CreateTenantAsync(
        CreateTenantRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantSummary>(
            () => CreateJsonRequest("kida/tenancy/tenants", request, cancellationToken),
            Method.POST,
            KidaTenancyScopes.Create,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantSummary> UpdateTenantAsync(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantSummary>(
            () => CreateJsonRequest($"kida/tenancy/tenants/{tenantId:D}", request, cancellationToken),
            Method.PUT,
            KidaTenancyScopes.Manage,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantAdministrationSnapshot> InspectTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantAdministrationSnapshot>(() => CreateRequest($"kida/tenancy/tenants/{tenantId:D}/details", cancellationToken), Method.GET, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantDomainInfo> AddTenantDomainAsync(AddTenantDomainRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantDomainInfo>(() => CreateJsonRequest($"kida/tenancy/tenants/{request.TenantId:D}/domains", request, cancellationToken), Method.POST, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask RemoveTenantDomainAsync(Guid tenantId, Guid domainId, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateRequest($"kida/tenancy/tenants/{tenantId:D}/domains/{domainId:D}", cancellationToken), Method.DELETE, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantMembershipInfo> SetTenantMembershipAsync(SetTenantMembershipRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantMembershipInfo>(() => CreateJsonRequest($"kida/tenancy/tenants/{request.TenantId:D}/memberships", request, cancellationToken), Method.PUT, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask RemoveTenantMembershipAsync(Guid tenantId, Guid membershipId, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateRequest($"kida/tenancy/tenants/{tenantId:D}/memberships/{membershipId:D}", cancellationToken), Method.DELETE, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantInvitationReceipt> CreateTenantInvitationAsync(CreateTenantInvitationRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantInvitationReceipt>(() => CreateJsonRequest($"kida/tenancy/tenants/{request.TenantId:D}/invitations", request, cancellationToken), Method.POST, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask RevokeTenantInvitationAsync(Guid tenantId, Guid invitationId, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateRequest($"kida/tenancy/tenants/{tenantId:D}/invitations/{invitationId:D}", cancellationToken), Method.DELETE, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantSettingInfo> SetTenantSettingAsync(SetTenantSettingRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantSettingInfo>(() => CreateJsonRequest($"kida/tenancy/tenants/{request.TenantId:D}/settings/{Uri.EscapeDataString(request.Key)}", request, cancellationToken), Method.PUT, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask RemoveTenantSettingAsync(Guid tenantId, string key, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateRequest($"kida/tenancy/tenants/{tenantId:D}/settings/{Uri.EscapeDataString(key)}", cancellationToken), Method.DELETE, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<IReadOnlyCollection<DeploymentInfo>> ListDeploymentsAsync(CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IReadOnlyCollection<DeploymentInfo>>(() => CreateRequest("kida/tenancy/deployments", cancellationToken), Method.GET, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<DeploymentInfo> CreateDeploymentAsync(CreateDeploymentRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<DeploymentInfo>(() => CreateJsonRequest("kida/tenancy/deployments", request, cancellationToken), Method.POST, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<TenantDeploymentInfo> AssignTenantDeploymentAsync(AssignTenantDeploymentRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TenantDeploymentInfo>(() => CreateJsonRequest($"kida/tenancy/tenants/{request.TenantId:D}/deployments", request, cancellationToken), Method.POST, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask UnassignTenantDeploymentAsync(Guid tenantId, Guid deploymentId, string familyCode, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateRequest($"kida/tenancy/tenants/{tenantId:D}/deployments/{deploymentId:D}/{Uri.EscapeDataString(familyCode)}", cancellationToken), Method.DELETE, KidaTenancyScopes.Manage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<EntitlementCatalogReceipt> RegisterEntitlementCatalogAsync(RegisterEntitlementCatalogRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<EntitlementCatalogReceipt>(() => CreateJsonRequest("kida/entitlements/catalog", request, cancellationToken), Method.POST, KidaEntitlementsScopes.CatalogRegister, cancellationToken).ConfigureAwait(false);

    public async ValueTask<EntitlementDecision> EvaluateEntitlementAsync(EvaluateEntitlementRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<EntitlementDecision>(() => CreateJsonRequest("kida/entitlements/evaluate", request, cancellationToken), Method.POST, KidaEntitlementsScopes.Evaluate, cancellationToken).ConfigureAwait(false);

    public async ValueTask<UsageReceipt> ReportEntitlementUsageAsync(ReportUsageRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<UsageReceipt>(() => CreateJsonRequest("kida/entitlements/usage", request, cancellationToken), Method.POST, KidaEntitlementsScopes.UsageReport, cancellationToken).ConfigureAwait(false);

    public async ValueTask<LicenseLeaseReceipt> AcquireLicenseAsync(AcquireLicenseRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<LicenseLeaseReceipt>(() => CreateJsonRequest("kida/entitlements/licenses/acquire", request, cancellationToken), Method.POST, KidaEntitlementsScopes.LicenseUse, cancellationToken).ConfigureAwait(false);

    public async ValueTask<LicenseLeaseReceipt> HeartbeatLicenseAsync(HeartbeatLicenseRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<LicenseLeaseReceipt>(() => CreateJsonRequest("kida/entitlements/licenses/heartbeat", request, cancellationToken), Method.POST, KidaEntitlementsScopes.LicenseUse, cancellationToken).ConfigureAwait(false);

    public async ValueTask ReleaseLicenseAsync(ReleaseLicenseRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateJsonRequest("kida/entitlements/licenses/release", request, cancellationToken), Method.POST, KidaEntitlementsScopes.LicenseUse, cancellationToken).ConfigureAwait(false);

    public async ValueTask<AppCatalogReceipt> RegisterAppCatalogAsync(RegisterAppCatalogRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AppCatalogReceipt>(() => CreateJsonRequest("kida/apps/catalog", request, cancellationToken), Method.POST, KidaAppsScopes.CatalogRegister, cancellationToken).ConfigureAwait(false);

    public async ValueTask<AppPage> SearchAppsAsync(string? query = null, string? status = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AppPage>(() => CreateRequest("kida/apps/catalog", cancellationToken).WithQuery(new QueryParam("query", query ?? string.Empty)).WithQuery(new QueryParam("status", status ?? string.Empty)).WithQuery(new QueryParam("page", Math.Max(page, 1).ToString(System.Globalization.CultureInfo.InvariantCulture))).WithQuery(new QueryParam("pageSize", Math.Clamp(pageSize, 1, 50).ToString(System.Globalization.CultureInfo.InvariantCulture))), Method.GET, KidaAppsScopes.CatalogRead, cancellationToken).ConfigureAwait(false);

    public async ValueTask<AppInstallationInfo> InstallAppAsync(InstallAppRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<AppInstallationInfo>(() => CreateJsonRequest("kida/apps/installations", request, cancellationToken), Method.POST, KidaAppsScopes.InstallationsManage, cancellationToken).ConfigureAwait(false);

    public async ValueTask<IReadOnlyCollection<AppInstallationInfo>> ListAppInstallationsAsync(Guid tenantId, string hostAudience, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<IReadOnlyCollection<AppInstallationInfo>>(() => CreateRequest("kida/apps/installations", cancellationToken).WithQuery(new QueryParam("tenantId", tenantId.ToString("D"))).WithQuery(new QueryParam("audience", hostAudience)), Method.GET, KidaAppsScopes.InstallationsRead, cancellationToken).ConfigureAwait(false);

    public async ValueTask<AppComposition> ComposeAppsAsync(Guid tenantId, Guid? projectId, string hostAudience, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest("kida/apps/composition", cancellationToken).WithQuery(new QueryParam("tenantId", tenantId.ToString("D"))).WithQuery(new QueryParam("audience", hostAudience));
        if (projectId is not null) request.WithQuery(new QueryParam("projectId", projectId.Value.ToString("D")));
        return await SendWithClientTokenAsync<AppComposition>(() => request, Method.GET, KidaAppsScopes.CompositionRead, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ReportAppHealthAsync(ReportAppHealthRequest request, CancellationToken cancellationToken = default) =>
        await SendWithClientTokenWithoutBodyAsync(() => CreateJsonRequest("kida/apps/health", request, cancellationToken), Method.POST, KidaAppsScopes.HealthReport, cancellationToken).ConfigureAwait(false);

    public async ValueTask<UserIdentity> GetCurrentUserAsync(
        string userAccessToken,
        CancellationToken cancellationToken = default) =>
        await SendWithUserTokenAsync<UserIdentity>(
            () => CreateRequest("kida/identity/me", cancellationToken),
            Method.GET,
            userAccessToken).ConfigureAwait(false);

    public async ValueTask<UserProfile> GetProfileAsync(
        string userAccessToken,
        CancellationToken cancellationToken = default) =>
        await SendWithUserTokenAsync<UserProfile>(
            () => CreateRequest("kida/identity/profile", cancellationToken),
            Method.GET,
            userAccessToken).ConfigureAwait(false);

    public async ValueTask<UserProfile> UpdateProfileAsync(
        string userAccessToken,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithUserTokenAsync<UserProfile>(
            () => CreateJsonRequest(
                "kida/identity/profile",
                request,
                cancellationToken),
            Method.PUT,
            userAccessToken).ConfigureAwait(false);

    public async ValueTask<IReadOnlyCollection<MfaMethodInfo>> ListMfaMethodsAsync(
        string userAccessToken,
        CancellationToken cancellationToken = default) =>
        await SendWithUserTokenAsync<IReadOnlyCollection<MfaMethodInfo>>(
            () => CreateRequest("kida/identity/mfa/methods", cancellationToken),
            Method.GET,
            userAccessToken).ConfigureAwait(false);

    public async ValueTask<TotpEnrollmentReceipt> BeginTotpEnrollmentAsync(
        string userAccessToken,
        BeginTotpEnrollmentRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithUserTokenAsync<TotpEnrollmentReceipt>(
            () => CreateJsonRequest("kida/identity/mfa/enrollments", request, cancellationToken),
            Method.POST,
            userAccessToken).ConfigureAwait(false);

    public async ValueTask<RecoveryCodesReceipt> ReplaceRecoveryCodesAsync(
        string userAccessToken,
        ReplaceRecoveryCodesRequest request,
        CancellationToken cancellationToken = default) =>
        await SendWithUserTokenAsync<RecoveryCodesReceipt>(
            () => CreateJsonRequest("kida/identity/mfa/recovery-codes", request, cancellationToken),
            Method.POST,
            userAccessToken).ConfigureAwait(false);

    private async Task<T> SendWithClientTokenAsync<T>(
        Func<IRequest> requestFactory,
        Method method,
        string requiredScope,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var request = requestFactory();
            var token = await clientTokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            EnsureClientScope(token, requiredScope);
            request.SetAuthenticator(CreateBearerProvider(token.AccessToken));
            var response = await request.SendAsync(method).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && attempt == 0)
            {
                response.OriginalResponse?.Dispose();
                clientTokenProvider.Invalidate(token.AccessToken);
                continue;
            }

            return await KidaResponseReader.ReadAsync<T>(response).ConfigureAwait(false);
        }

        throw new InvalidOperationException("The Kida request retry loop ended unexpectedly.");
    }

    private async Task<T> SendAnonymousAsync<T>(string path, object body, CancellationToken cancellationToken)
    {
        var response = await CreateJsonRequest(path, body, cancellationToken).DoNotAuthenticate().PostAsync().ConfigureAwait(false);
        return await KidaResponseReader.ReadAsync<T>(response).ConfigureAwait(false);
    }

    private async Task SendWithClientTokenWithoutBodyAsync(
        Func<IRequest> requestFactory,
        Method method,
        string requiredScope,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var request = requestFactory();
            var token = await clientTokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            EnsureClientScope(token, requiredScope);
            request.SetAuthenticator(CreateBearerProvider(token.AccessToken));
            var response = await request.SendAsync(method).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized && attempt == 0)
            {
                response.OriginalResponse?.Dispose();
                clientTokenProvider.Invalidate(token.AccessToken);
                continue;
            }

            await KidaResponseReader.EnsureSuccessAsync(response).ConfigureAwait(false);
            return;
        }

        throw new InvalidOperationException("The Kida request retry loop ended unexpectedly.");
    }

    private static async Task<T> SendWithUserTokenAsync<T>(
        Func<IRequest> requestFactory,
        Method method,
        string userAccessToken)
    {
        EnsureUserToken(userAccessToken);
        var request = requestFactory().SetAuthenticator(CreateBearerProvider(userAccessToken));
        var response = await request.SendAsync(method).ConfigureAwait(false);
        return await KidaResponseReader.ReadAsync<T>(response).ConfigureAwait(false);
    }

    private static async Task SendWithUserTokenAsync(
        Func<IRequest> requestFactory,
        Method method,
        string userAccessToken)
    {
        EnsureUserToken(userAccessToken);
        var request = requestFactory().SetAuthenticator(CreateBearerProvider(userAccessToken));
        var response = await request.SendAsync(method).ConfigureAwait(false);
        await KidaResponseReader.EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async ValueTask<ClientTokenResult> IssueProductClientTokenAsync(
        string clientIdentifier,
        string clientSecret,
        CancellationToken cancellationToken = default)
    {
        var request = new ClientTokenRequest(
            clientIdentifier,
            clientSecret,
            _options.UserAudience);
        var response = await CreateJsonRequest(
                "kida/identity/client-tokens",
                request,
                cancellationToken)
            .AddHeader(KidaIdentityHeaders.ClientIdentifier, clientIdentifier.Trim())
            .DoNotAuthenticate()
            .PostAsync()
            .ConfigureAwait(false);
        return await KidaResponseReader.ReadAsync<ClientTokenResult>(response).ConfigureAwait(false);
    }

    public async ValueTask<PasswordChangeReceipt?> ChangePasswordAtEdgeAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var boundRequest = request with { ClientIdentifier = _options.ClientIdentifier };
        var response = await AddIdentityHeaders(
                CreateJsonRequest("kida/identity/password/change", boundRequest, cancellationToken),
                username: boundRequest.Username,
                includeClientIdentifier: false)
            .DoNotAuthenticate()
            .PostAsync()
            .ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            response.OriginalResponse?.Dispose();
            return null;
        }

        return await KidaResponseReader.ReadAsync<PasswordChangeReceipt>(response).ConfigureAwait(false);
    }

    public async ValueTask<PasswordResetGrantReceipt> VerifyPasswordResetAsync(
        Guid challengeId,
        string code,
        string? returnUri = null,
        string? state = null,
        CancellationToken cancellationToken = default)
    {
        var request = new VerifyPasswordResetCodeRequest(
            challengeId,
            _options.ClientId,
            _options.UserAudience,
            code,
            returnUri,
            state);
        var response = await CreateJsonRequest(
                "kida/identity/password/resets/verify",
                request,
                cancellationToken)
            .AddHeader(KidaIdentityHeaders.Challenge, challengeId.ToString("D"))
            .DoNotAuthenticate()
            .PostAsync()
            .ConfigureAwait(false);
        return await KidaResponseReader.ReadAsync<PasswordResetGrantReceipt>(response).ConfigureAwait(false);
    }

    public async ValueTask<PasswordResetCompletionReceipt> CompletePasswordResetAsync(
        Guid grantId,
        string newPassword,
        string? returnUri = null,
        string? state = null,
        CancellationToken cancellationToken = default) =>
        await SendAnonymousAsync<PasswordResetCompletionReceipt>(
            "kida/identity/password/resets/complete",
            new CompletePasswordResetRequest(
                grantId,
                _options.ClientId,
                _options.UserAudience,
                newPassword,
                returnUri,
                state),
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<SamlAuthenticationStart> BeginSamlAuthenticationAsync(
        string providerCode,
        string returnUri,
        string state,
        string codeChallenge,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<SamlAuthenticationStart>(
            () => CreateJsonRequest(
                "kida/identity/saml/start",
                new BeginSamlAuthenticationRequest(
                    _options.ClientId,
                    _options.UserAudience,
                    providerCode,
                    returnUri,
                    state,
                    codeChallenge),
                cancellationToken),
            Method.POST,
            KidaIdentityScopes.Authenticate,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<SamlAuthenticationHandoff> CompleteSamlAuthenticationAsync(
        string samlResponse,
        string relayState,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<SamlAuthenticationHandoff>(
            () => CreateJsonRequest(
                "kida/identity/saml/complete",
                new CompleteSamlAuthenticationRequest(samlResponse, relayState, _options.ClientId),
                cancellationToken),
            Method.POST,
            KidaIdentityScopes.Authenticate,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<TotpEnrollmentDetails> InspectTotpEnrollmentAsync(
        string ticket,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TotpEnrollmentDetails>(
            () => CreateRequest("kida/identity/mfa/enrollments/inspect", cancellationToken)
                .WithQuery(new QueryParam("ticket", ticket)),
            Method.GET,
            KidaIdentityScopes.Authenticate,
            cancellationToken).ConfigureAwait(false);

    public async ValueTask<TotpEnrollmentCompletion> ConfirmTotpEnrollmentAsync(
        string ticket,
        string code,
        CancellationToken cancellationToken = default) =>
        await SendWithClientTokenAsync<TotpEnrollmentCompletion>(
            () => CreateJsonRequest(
                "kida/identity/mfa/enrollments/confirm",
                new ConfirmTotpTicketRequest(ticket, code),
                cancellationToken),
            Method.POST,
            KidaIdentityScopes.Authenticate,
            cancellationToken).ConfigureAwait(false);

    private IRequest CreateRequest(string path, CancellationToken cancellationToken) =>
        transport.Client
            .WithEndPoint(path)
            .AddCancellationToken(cancellationToken);

    private IRequest CreateJsonRequest<T>(
        string path,
        T body,
        CancellationToken cancellationToken) =>
        CreateRequest(path, cancellationToken)
            .WithBody(new RawBodyRequestContent(body!));

    private IRequest AddIdentityHeaders(
        IRequest request,
        string? username = null,
        bool includeClientIdentifier = true)
    {
        if (includeClientIdentifier)
        {
            request.AddHeader(KidaIdentityHeaders.ClientIdentifier, _options.ClientIdentifier);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            request.AddHeader(KidaIdentityHeaders.Username, username.Trim());
        }

        return request;
    }

    private static IAuthProvider CreateBearerProvider(string accessToken) =>
        new TokenAuthProvider().SetToken(accessToken);

    private static void EnsureUserToken(string userAccessToken)
    {
        if (string.IsNullOrWhiteSpace(userAccessToken))
        {
            throw new ArgumentException("A user access token is required.", nameof(userAccessToken));
        }
    }

    private static void EnsureClientScope(KidaClientTokenLease token, string requiredScope)
    {
        if (token.Scopes.Contains(requiredScope)) return;
        throw new KidaRequestException(
            $"Kida issued the client token without required scope '{requiredScope}'.",
            HttpStatusCode.Forbidden,
            IdentityErrorCodes.InvalidClientScope);
    }

}
