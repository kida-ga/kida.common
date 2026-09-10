using System.Text;

namespace Kida.Models;
public sealed record AccessDecision(bool Allowed, string ReasonCode, IReadOnlyCollection<Guid> MatchedAssignmentIds);
