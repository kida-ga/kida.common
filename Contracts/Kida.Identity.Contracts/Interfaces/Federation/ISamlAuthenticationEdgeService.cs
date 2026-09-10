using Haley.Abstractions;

namespace Kida.Abstractions;
public interface ISamlAuthenticationEdgeService
{
    ValueTask<IFeedback<SamlAuthenticationStart>> BeginAsync(BeginSamlAuthenticationRequest request, CancellationToken cancellationToken = default);
    ValueTask<IFeedback<SamlAuthenticationHandoff>> CompleteAsync(CompleteSamlAuthenticationRequest request, CancellationToken cancellationToken = default);
}
