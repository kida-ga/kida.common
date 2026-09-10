using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserIdentityPage(IReadOnlyCollection<UserIdentity> Users, int Page, int PageSize, long TotalCount);
