namespace Kida.Models;

public sealed record AppCatalogReceipt(Guid AppId, Guid VersionId, string Code, string Version, string ContentHash, string Status);
