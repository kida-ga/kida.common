using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserIdentity(Guid UserId, string DisplayName, IdentityStatus Status, string? Username, DateTimeOffset CreatedAt, DateTimeOffset? LastAuthenticatedAt, bool PasswordChangeRequired = false);
