using Kida.Utils;
using Haley.Abstractions;
using Haley.Utils;
using System.Text.Json;

namespace Kida.Service.Client;

internal static class KidaResponseReader
{
    private static readonly JsonSerializerOptions ClientJson = ClientIdentityJson.CreateOptions(ObjectSerialization.GenerateNewOptions(true));

    internal static async Task<T> ReadAsync<T>(IResponse response)
    {
        try
        {
            var content = (await response.AsStringResponseAsync().ConfigureAwait(false)).Content;
            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new KidaRequestException("Kida returned an empty response.", response.StatusCode);
                }

                return content.FromJson<T>(ClientJson)
                    ?? throw new KidaRequestException("Kida returned an empty response.", response.StatusCode);
            }

            var problem = TryReadProblem(content);
            throw Rejected(response, problem.ErrorCode, problem.Detail, problem.TraceId);
        }
        finally
        {
            response.OriginalResponse?.Dispose();
        }
    }

    internal static async Task EnsureSuccessAsync(IResponse response)
    {
        try
        {
            if (response.IsSuccessStatusCode) return;
            var content = (await response.AsStringResponseAsync().ConfigureAwait(false)).Content;
            var problem = TryReadProblem(content);
            throw Rejected(response, problem.ErrorCode, problem.Detail, problem.TraceId);
        }
        finally
        {
            response.OriginalResponse?.Dispose();
        }
    }

    private static KidaRequestException Rejected(IResponse response, string? errorCode, string? detail, string? traceId) =>
        new(
            string.IsNullOrWhiteSpace(detail)
                ? $"Kida rejected the request with HTTP {(int)response.StatusCode}."
                : $"Kida rejected the request with HTTP {(int)response.StatusCode}: {detail}",
            response.StatusCode,
            errorCode,
            detail,
            traceId);

    private static (string? ErrorCode, string? Detail, string? TraceId) TryReadProblem(string? content)
    {
        if (string.IsNullOrWhiteSpace(content) || !content.TrimStart().StartsWith('{'))
        {
            return default;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return default;
            }

            var root = document.RootElement;
            var errorCode = ReadString(root, "code");
            var detail = ReadString(root, "detail") ?? ReadString(root, "title");
            return (errorCode, detail, ReadString(root, "traceId"));
        }
        catch (JsonException)
        {
            return default;
        }
    }

    private static string? ReadString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        var content = value.GetString();
        return string.IsNullOrWhiteSpace(content) ? null : content;
    }
}
