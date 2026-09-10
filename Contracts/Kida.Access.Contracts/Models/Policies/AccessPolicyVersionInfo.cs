using System.Text;

namespace Kida.Models;
public sealed record AccessPolicyVersionInfo(Guid TenantId, string Module, ulong Version, DateTimeOffset? ModifiedAt, string? RevisionHash = null, string Resource = "");
