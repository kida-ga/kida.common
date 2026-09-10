using Haley.Abstractions;
using Haley.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

/// <summary>
/// Resolves the process-wide Kida outbound client through Haley.Rest ClientStore.
/// </summary>
internal sealed class KidaClientTransport
{
    internal KidaClientTransport(IClient client) =>
        Client = client ?? throw new ArgumentNullException(nameof(client));

    public KidaClientTransport(
        IOptions<KidaClientOptions> options,
        ILogger<KidaClientTransport> logger)
    {
        var value = options.Value;
        var storeKey = $"{typeof(KidaClientTransport).FullName}::{value.ClientIdentifier}::{value.Url}";

        Client = ClientStore.Get(storeKey)
            ?? ClientStore.AddClient(storeKey, value.Url, logger)
            ?? throw new InvalidOperationException("Haley ClientStore could not create the Kida service client.");
        Client.WithTimeOut(TimeSpan.FromSeconds(30));
    }

    internal IClient Client { get; }
}
