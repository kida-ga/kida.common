using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserSessionSearchRequest(Guid UserId, bool ActiveOnly = true, int Page = 1, int PageSize = 10);
