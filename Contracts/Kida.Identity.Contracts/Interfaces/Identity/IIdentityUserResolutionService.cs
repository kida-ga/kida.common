using Haley.Models;
using Haley.Abstractions;

namespace Kida.Abstractions;
/// <summary>
/// Exact-username lookup for explicitly authorized product bootstrap clients.
/// This deliberately does not expose a searchable global directory.
/// </summary>
public interface IIdentityUserResolutionService
{
    ValueTask<UserIdentity?> ResolveUserAsync(string username, CancellationToken cancellationToken = default);
}
