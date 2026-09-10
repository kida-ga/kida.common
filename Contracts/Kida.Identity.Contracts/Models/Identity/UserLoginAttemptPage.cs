namespace Kida.Models;

public sealed record UserLoginAttemptPage(
    IReadOnlyCollection<UserLoginAttemptInfo> Attempts,
    int Page,
    int PageSize,
    long TotalCount);
