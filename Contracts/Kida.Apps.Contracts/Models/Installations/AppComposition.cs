namespace Kida.Models;

public sealed record AppComposition(
    Guid TenantId,
    Guid? ProjectId,
    string HostAudience,
    IReadOnlyCollection<AppInstallationInfo> Installations,
    IReadOnlyCollection<AppEndpointDeclaration> Endpoints,
    IReadOnlyCollection<AppContributionDeclaration> Contributions,
    string RevisionHash);
