using Haley.Abstractions;

namespace Kida.Models;
public sealed record CompleteIdentityEnrollmentRequest(Guid GrantId, Guid ClientId, Guid UserId, string Password, string Context);
