namespace Kida.Models;

public sealed record RegisterAppPublisherRequest(
    string Code,
    string DisplayName,
    string? Description,
    string Type,
    Guid? PublisherTenantId,
    string PublisherAudience,
    string KeyId,
    string PublicKeyPem);
