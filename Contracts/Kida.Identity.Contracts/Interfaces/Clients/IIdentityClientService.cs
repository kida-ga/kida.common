using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Abstractions;
public interface IIdentityClientService
{
    ValueTask<ClientTokenResult> IssueClientTokenAsync(ClientTokenRequest request, CancellationToken cancellationToken = default);
}
