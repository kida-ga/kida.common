namespace Kida.Models;
public sealed class KidaScopeMetadataFile
{
    public string? ModuleDisplayName { get; set; }
    public string? ModuleDescription { get; set; }
    public KidaScopeMetadata[] Scopes { get; set; } = [];
    public KidaActionMetadata[] Actions { get; set; } = [];
}
