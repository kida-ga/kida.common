namespace Kida.Abstractions;
public interface IIdentityOperationsService
{
    ValueTask<IdentityOperationalSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
    ValueTask<IdentityOperationalPage> ListAsync(IdentityOperationalQuery query, CancellationToken cancellationToken = default);
    ValueTask<IdentityOperationalExportProbe> ProbeExportAsync(IdentityOperationalQuery query, CancellationToken cancellationToken = default);
}
