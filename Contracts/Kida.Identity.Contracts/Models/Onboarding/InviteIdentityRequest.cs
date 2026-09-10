using Haley.Abstractions;

namespace Kida.Models;
public sealed record InviteIdentityRequest(Guid ClientId, string Resource, string Email, string DisplayName, Guid? TenantId = null);
