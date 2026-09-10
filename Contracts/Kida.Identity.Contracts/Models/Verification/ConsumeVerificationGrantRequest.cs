using Haley.Abstractions;

namespace Kida.Models;
public sealed record ConsumeVerificationGrantRequest(Guid GrantId, Guid ClientId, string Purpose, Guid? SubjectId, string Context, Guid ConsumedById);
