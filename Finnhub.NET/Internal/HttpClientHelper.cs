using System.Net.Http.Headers;
using System.Text.Json;

namespace Finnhub.NET.Internal;

internal sealed class HttpClientHelper
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public HttpClientHelper(HttpClient httpClient, JsonSerializerOptions serializerOptions)
    {
        _httpClient = httpClient;
        _serializerOptions = serializerOptions;
    }

    public Task<T> GetAsync<T>(string pathAndQuery, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, pathAndQuery);
        return SendAsync<T>(request, cancellationToken);
    }

    public Task<T> PostAsync<T>(string pathAndQuery, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, pathAndQuery);
        request.Content = content;

        return SendAsync<T>(request, cancellationToken);
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if ((int)response.StatusCode == 429)
        {
            throw new RateLimitException(
                "Finnhub API rate limit exceeded.",
                responseBody,
                GetRetryAfter(response.Headers.RetryAfter));
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new FinnhubException(
                $"Finnhub API request failed with status code {(int)response.StatusCode}.",
                (int)response.StatusCode,
                responseBody);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)responseBody;
        }

        try
        {
            var value = JsonSerializer.Deserialize<T>(responseBody, _serializerOptions);
            if (value is null)
            {
                throw new FinnhubException(
                    "Finnhub API returned an empty response body.",
                    (int)response.StatusCode,
                    responseBody);
            }

            return value;
        }
        catch (JsonException ex)
        {
            throw new FinnhubException(
                "Failed to deserialize Finnhub API response.",
                (int)response.StatusCode,
                responseBody,
                ex);
        }
    }

    private static TimeSpan? GetRetryAfter(RetryConditionHeaderValue? retryAfter)
    {
        if (retryAfter is null)
        {
            return null;
        }

        if (retryAfter.Delta.HasValue)
        {
            return retryAfter.Delta.Value;
        }

        if (retryAfter.Date.HasValue)
        {
            var wait = retryAfter.Date.Value - DateTimeOffset.UtcNow;
            return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
        }

        return null;
    }
}
