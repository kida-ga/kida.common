using Haley.Abstractions;

namespace Kida.Models;
public sealed record BootstrapIdentityRequest(Guid ClientId, string Username, string DisplayName, string InitialPassword, string SourceReference, bool RequirePasswordChange = true, Guid? TenantId = null, string Resource = "");
