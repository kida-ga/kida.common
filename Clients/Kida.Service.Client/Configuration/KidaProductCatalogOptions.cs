namespace Kida.Service.Client;

public sealed class KidaProductCatalogOptions
{
    public const string DefaultSectionName = "Kida:ProductCatalog";

    public bool RegisterOnStartup { get; set; } = true;
    public string ApplicationInfoPath { get; set; } = "appinfo.json";
    public string? BaseDirectory { get; set; }
    public string? DeploymentInfoLocation { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
