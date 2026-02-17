using Finnhub.NET.Internal;
using Microsoft.Extensions.Options;

namespace Finnhub.NET;

public sealed class FinnhubClient : IFinnhubClient
{
    private const string ApiKeyHeaderName = "X-Finnhub-Token";

    private readonly HttpClient _httpClient;
    private readonly HttpClientHelper _httpClientHelper;

    public FinnhubClient(HttpClient httpClient, IOptions<FinnhubOptions> options)
        : this(httpClient, options.Value)
    {
    }

    public FinnhubClient(HttpClient httpClient, FinnhubOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException("Finnhub API key must be configured.", nameof(options));
        }

        _httpClient = httpClient;
        ConfigureHttpClient(_httpClient, options);
        _httpClientHelper = new HttpClientHelper(_httpClient, FinnhubJson.Default);
    }

    internal Task<TResponse> GetFromApiAsync<TResponse>(
        string path,
        IReadOnlyDictionary<string, string?>? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var pathAndQuery = QueryStringBuilder.Build(path, queryParameters);
        return _httpClientHelper.GetAsync<TResponse>(pathAndQuery, cancellationToken);
    }

    internal Task<TResponse> PostToApiAsync<TResponse>(
        string path,
        HttpContent? content = null,
        IReadOnlyDictionary<string, string?>? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var pathAndQuery = QueryStringBuilder.Build(path, queryParameters);
        return _httpClientHelper.PostAsync<TResponse>(pathAndQuery, content, cancellationToken);
    }

    internal static void ConfigureHttpClient(HttpClient httpClient, FinnhubOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        }

        if (options.Timeout > TimeSpan.Zero)
        {
            httpClient.Timeout = options.Timeout;
        }

        if (httpClient.DefaultRequestHeaders.Contains(ApiKeyHeaderName))
        {
            httpClient.DefaultRequestHeaders.Remove(ApiKeyHeaderName);
        }

        httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, options.ApiKey);
    }
}
