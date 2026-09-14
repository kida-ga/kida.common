namespace Kida.Models;

public sealed record RoleAssignmentPage(
    IReadOnlyCollection<RoleAssignmentInfo> Assignments,
    int Page,
    int PageSize,
    bool HasNext);
