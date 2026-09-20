using System.Text.Json.Serialization;
namespace Kida.Models;

public sealed record SubjectAssignmentSearchRequest(
    Guid SubjectId,
    string? SubjectType = null,
    [property: JsonConverter(typeof(JsonNumberEnumConverter<AccessStatus>))] AccessStatus? Status = null,
    int Page = 1,
    int PageSize = 20);
