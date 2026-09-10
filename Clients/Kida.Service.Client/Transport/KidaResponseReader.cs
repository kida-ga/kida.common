using Haley.Abstractions;
using Haley.Utils;

namespace Kida.Service.Client;

internal static class KidaResponseReader
{
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

                return content.FromJson<T>()
                    ?? throw new KidaRequestException("Kida returned an empty response.", response.StatusCode);
            }

            throw Rejected(response, TryReadErrorCode(content));
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
            throw Rejected(response, TryReadErrorCode(content));
        }
        finally
        {
            response.OriginalResponse?.Dispose();
        }
    }

    private static KidaRequestException Rejected(IResponse response, string? errorCode) =>
        new(
            $"Kida rejected the request with HTTP {(int)response.StatusCode}.",
            response.StatusCode,
            errorCode);

    private static string? TryReadErrorCode(string? content)
    {
        if (string.IsNullOrWhiteSpace(content) ||
            !content.TrimStart().StartsWith('{') ||
            !content.IsValidJson(tryParse: true)) return null;

        var values = content.FromJson<Dictionary<string, object>>();
        return values is not null && values.TryGetValue("code", out var code)
            ? code?.ToString()
            : null;
    }
}
