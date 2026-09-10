namespace Kida.Models;
/// <summary>
/// Describes the service/resource audiences exposed by the current host. Modules
/// are discovered from <see cref = "IKidaModuleCatalogProvider"/> registrations and
/// must never be repeated in application configuration.
/// </summary>
public sealed class KidaScopeCatalogOptions
{
    public bool RegisterOnStartup { get; set; } = true;
    public string MetadataRoot { get; set; } = "Config/Scopes";
    public bool OverwriteDescriptions { get; set; } = true;
    public string Audience { get; set; } = string.Empty;
    public string[] Audiences { get; set; } = [];

    public IReadOnlyCollection<string> GetAudiences() => new[]
    {
        Audience
    }.Concat(Audiences ?? []).Select(value => value?.Trim() ?? string.Empty).Where(value => value.Length > 0).Distinct(StringComparer.Ordinal).ToArray();
}
