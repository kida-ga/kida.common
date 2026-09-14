namespace Kida.Models;

public sealed record SubjectAssignmentSearchRequest(
    Guid SubjectId,
    string? SubjectType = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20);
