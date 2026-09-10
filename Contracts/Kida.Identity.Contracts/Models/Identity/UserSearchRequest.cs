using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserSearchRequest(string? Query = null, IdentityStatus? Status = null, int Limit = 100, Guid? AfterUserId = null);
