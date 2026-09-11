namespace Kida.Models;

public sealed record EntitlementCatalogReceipt(
    Guid ProductId,
    string ProductCode,
    string Audience,
    long Revision,
    string RevisionHash,
    int CreatedFeatures,
    int CreatedMeters);
