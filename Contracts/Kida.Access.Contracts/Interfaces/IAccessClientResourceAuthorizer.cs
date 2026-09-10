using System.Text;

namespace Kida.Abstractions;
/// <summary>
/// Authorizes a client against Kida's durable client → service audience → module
/// boundary. Implementations are supplied by a host that composes Identity and Access.
/// </summary>
public interface IAccessClientResourceAuthorizer
{
    /// <summary>
    /// Applies composition-owned catalog namespace rules before a client can
    /// publish actions and scope mappings. Standalone Access hosts may retain
    /// the default; Kida Service supplies the strict Kida boundary.
    /// </summary>
    bool MayPublishCatalog(RegisterModuleRequest request) => true;
    ValueTask<bool> IsAuthorizedAsync(Guid clientId, string resource, CancellationToken cancellationToken = default);
    ValueTask<bool> IsAuthorizedAsync(Guid clientId, string resource, string moduleCode, string? moduleVersion = null, CancellationToken cancellationToken = default);
}
