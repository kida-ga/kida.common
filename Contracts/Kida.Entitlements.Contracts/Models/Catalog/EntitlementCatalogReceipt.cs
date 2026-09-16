namespace Kida.Models;

public sealed record EntitlementCatalogReceipt(
    Guid ProductId,
    Guid ReleaseId,
    Guid DeploymentId,
    string ProductCode,
    string Audience,
    string Version,
    long Revision,
    string RevisionHash,
    int CreatedFeatures,
    int CreatedMeters);
