using System.Text;

namespace Kida.Models;
public sealed record AccessResult(bool Succeeded, Guid? ResourceId = null, string? ErrorCode = null)
{
    public static AccessResult Success(Guid? resourceId = null) => new(true, resourceId);
    public static AccessResult Failure(string errorCode) => new(false, ErrorCode: errorCode);
}
