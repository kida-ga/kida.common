using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserSessionPage(IReadOnlyCollection<UserSessionInfo> Sessions, int Page, int PageSize, bool HasNext);
