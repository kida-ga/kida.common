using Haley.Abstractions;
using Haley.Models;
using Haley.Utils;
using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;
/// <summary>
/// Process-wide cache for the confidential client's short-lived Kida access token.
/// <see cref = "IKidaClient"/> instances are transient; this provider is a singleton.
/// </summary>
public interface IKidaClientTokenProvider
{
    ValueTask<KidaClientTokenLease> GetTokenAsync(CancellationToken cancellationToken = default);
    ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    void Invalidate(string rejectedAccessToken);
}
