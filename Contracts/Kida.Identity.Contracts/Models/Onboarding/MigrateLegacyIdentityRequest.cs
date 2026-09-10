using Haley.Abstractions;

namespace Kida.Models;
public sealed record MigrateLegacyIdentityRequest(Guid ClientId, string Username, string DisplayName, string SourceReference, string? Email = null, Guid? TenantId = null, string Resource = "");
