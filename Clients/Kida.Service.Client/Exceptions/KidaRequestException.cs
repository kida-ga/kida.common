using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
namespace Kida.Service.Client;
public sealed class KidaRequestException : HttpRequestException
{
    public KidaRequestException(
        string message,
        System.Net.HttpStatusCode statusCode,
        string? errorCode = null)
        : this(message, statusCode, errorCode, null, null)
    {
    }

    public KidaRequestException(
        string message,
        System.Net.HttpStatusCode statusCode,
        string? errorCode,
        string? responseDetail)
        : this(message, statusCode, errorCode, responseDetail, null)
    {
    }

    public KidaRequestException(
        string message,
        System.Net.HttpStatusCode statusCode,
        string? errorCode,
        string? responseDetail,
        string? responseTraceId)
        : base(message, null, statusCode)
    {
        ErrorCode = errorCode;
        ResponseDetail = responseDetail;
        ResponseTraceId = responseTraceId;
    }

    public string? ErrorCode { get; }
    public string? ResponseDetail { get; }
    public string? ResponseTraceId { get; }
}
