using Haley.Abstractions;

namespace Kida.Abstractions;
public interface IIdentityProvisioningService
{
    ValueTask<IFeedback<ProvisionedUser>> ProvisionLocalUserAsync(ProvisionLocalUserRequest request, CancellationToken cancellationToken = default);
}
