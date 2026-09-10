using Haley.Abstractions;

namespace Kida.Models;
public sealed record UserPageRequest(string? Query = null, IdentityStatus? Status = null, int Page = 1, int PageSize = 20, UserActivityFilter Activity = UserActivityFilter.All, UserSortOrder Sort = UserSortOrder.CreatedNewest);
