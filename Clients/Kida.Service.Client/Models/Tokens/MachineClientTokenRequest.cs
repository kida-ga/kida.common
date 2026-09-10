namespace Kida.Service.Client;

internal sealed record MachineClientTokenRequest(
    string ClientIdentifier,
    string ClientSecret);
