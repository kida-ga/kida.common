using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
namespace Kida.Service.Client;
public sealed class KidaRequestException(string message, System.Net.HttpStatusCode statusCode, string? errorCode = null) : HttpRequestException(message, null, statusCode)
{
    public string? ErrorCode { get; } = errorCode;
}
