using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityPolicyAdministrationService
{
    ValueTask<IReadOnlyCollection<IdentityClientPolicy>> ListClientPoliciesAsync(CancellationToken cancellationToken = default);
    ValueTask<IFeedback> UpsertClientPolicyAsync(Guid clientId, string resource, UpsertIdentityClientPolicyRequest request, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyCollection<UserMfaPolicyOverride>> ListUserMfaPolicyOverridesAsync(Guid userId, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> SetUserMfaPolicyOverrideAsync(Guid userId, Guid clientId, string resource, SetUserMfaPolicyOverrideRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback> DeleteUserMfaPolicyOverrideAsync(Guid userId, Guid clientId, string resource, CancellationToken cancellationToken = default);
}
