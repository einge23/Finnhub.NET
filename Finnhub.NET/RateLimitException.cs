namespace Finnhub.NET;

public sealed class RateLimitException(string message, string responseBody, TimeSpan? retryAfter = null)
    : FinnhubException(message, 429, responseBody)
{
    public TimeSpan? RetryAfter { get; } = retryAfter;
}
