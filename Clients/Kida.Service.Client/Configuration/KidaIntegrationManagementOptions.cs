namespace Kida.Service.Client;

/// <summary>
/// Credentials for the product's dedicated delegated client registrar. This is a
/// trusted Service client that creates Confidential integrations for one fixed product
/// audience; it must not use the ordinary runtime client credentials.
/// </summary>
public sealed class KidaIntegrationManagementOptions
{
    public const string DefaultSectionName = "Kida:IntegrationManagement";

    public Guid ClientId { get; set; }
    public string ClientIdentifier { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
