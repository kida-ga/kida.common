using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Haley.Abstractions;
using Haley.Rest;
using Haley.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kida.ResourceServer;
internal sealed class KidaJwksTransport
{
    internal KidaJwksTransport(IClient client, string endpoint)
    {
        Client = client;
        Endpoint = endpoint;
    }

    public KidaJwksTransport(IOptions<KidaResourceServerOptions> options, ILogger<KidaJwksTransport> logger)
    {
        var uri = options.Value.JwksUri ?? throw new InvalidOperationException("Kida resource-server JwksUri is required.");
        var origin = uri.GetLeftPart(UriPartial.Authority);
        var storeKey = $"{typeof(KidaJwksTransport).FullName}::{uri.AbsoluteUri}";
        Client = ClientStore.Get(storeKey) ?? ClientStore.AddClient(storeKey, origin, "Kida signing-key discovery", logger: logger) ?? throw new InvalidOperationException("Haley ClientStore could not create the Kida JWKS client.");
        Client.WithTimeOut(TimeSpan.FromSeconds(15));
        Endpoint = uri.PathAndQuery.TrimStart('/');
    }

    internal IClient Client { get; }
    internal string Endpoint { get; }
}
