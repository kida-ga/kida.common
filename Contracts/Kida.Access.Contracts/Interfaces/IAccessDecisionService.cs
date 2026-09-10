using System.Text;

namespace Kida.Abstractions;
public interface IAccessDecisionService
{
    ValueTask<AccessDecision> EvaluateAsync(AccessDecisionRequest request, CancellationToken cancellationToken = default);
}
