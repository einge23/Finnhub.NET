using Finnhub.NET.Internal;
using Finnhub.NET.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

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
        var pathAndQuery = QueryStringBuilder.Build(NormalizePath(path), queryParameters);
        return _httpClientHelper.GetAsync<TResponse>(pathAndQuery, cancellationToken);
    }

    internal Task<TResponse> PostToApiAsync<TResponse>(
        string path,
        HttpContent? content = null,
        IReadOnlyDictionary<string, string?>? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var pathAndQuery = QueryStringBuilder.Build(NormalizePath(path), queryParameters);
        return _httpClientHelper.PostAsync<TResponse>(pathAndQuery, content, cancellationToken);
    }

    internal static void ConfigureHttpClient(HttpClient httpClient, FinnhubOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            httpClient.BaseAddress = new Uri(NormalizeBaseUrl(options.BaseUrl), UriKind.Absolute);
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
    
    /// <summary>
    /// Get real-time quote data for US stocks. Constant polling is not recommended. Use websocket if you need real-time updates.
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns cref="Quote">Quote data</returns>
    public Task<Quote> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
    {
        symbol = ValidateRequired(symbol, nameof(symbol));
        var parameters = new Dictionary<string, string?> { ["symbol"] = symbol };
        return GetFromApiAsync<Quote>("/quote", parameters, cancellationToken);
    }

    public Task<CompanyProfile> GetCompanyProfileAsync(
        string? symbol = null,
        string? isin = null,
        string? cusip = null,
        CancellationToken ct = default)
    {
        ValidateAtLeastOneIdentifier(symbol, isin, cusip);
        symbol = NormalizeOptional(symbol);
        isin = NormalizeOptional(isin);
        cusip = NormalizeOptional(cusip);

        var parameters = new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
            ["isin"] = isin,
            ["cusip"] = cusip
        };
        return GetFromApiAsync<CompanyProfile>("/stock/profile", parameters, ct);
    }

    public Task<CompanyProfile2> GetCompanyProfile2Async(
        string? symbol = null,
        string? isin = null,
        string? cusip = null,
        CancellationToken ct = default)
    {
        ValidateAtLeastOneIdentifier(symbol, isin, cusip);
        symbol = NormalizeOptional(symbol);
        isin = NormalizeOptional(isin);
        cusip = NormalizeOptional(cusip);

        var parameters = new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
            ["isin"] = isin,
            ["cusip"] = cusip
        };
        return GetFromApiAsync<CompanyProfile2>("/stock/profile2", parameters, ct);
    }

    public Task<StockCandles> GetStockCandlesAsync(
        string symbol,
        string resolution,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct = default)
    {
        symbol = ValidateRequired(symbol, nameof(symbol));
        resolution = ValidateRequired(resolution, nameof(resolution));

        if (from > to)
        {
            throw new ArgumentException("'from' must be less than or equal to 'to'.", nameof(from));
        }

        var parameters = new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
            ["resolution"] = resolution,
            ["from"] = from.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
            ["to"] = to.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)
        };
        return GetFromApiAsync<StockCandles>("/stock/candle", parameters, ct);
    }

    public Task<IReadOnlyList<CompanyNews>> GetCompanyNewsAsync(
        string symbol,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        symbol = ValidateRequired(symbol, nameof(symbol));
        if (from > to)
        {
            throw new ArgumentException("'from' must be less than or equal to 'to'.", nameof(from));
        }

        var parameters = new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
            ["from"] = from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["to"] = to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
        return GetFromApiAsync<IReadOnlyList<CompanyNews>>("/company-news", parameters, ct);
    }

    public Task<IReadOnlyList<string>> GetStockPeersAsync(string symbol, string? grouping, CancellationToken ct = default)
    {
        symbol = ValidateRequired(symbol, nameof(symbol));
        grouping = NormalizeOptional(grouping);

        var parameters = new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
            ["grouping"] = grouping
        };
        return GetFromApiAsync<IReadOnlyList<string>>("/stock/peers", parameters, ct);
    }

    public Task<IReadOnlyList<StockSymbol>> GetStockSymbolsAsync(
        string exchange,
        string? mic,
        string? securityType,
        string? currency,
        CancellationToken ct = default)
    {
        exchange = ValidateRequired(exchange, nameof(exchange));
        mic = NormalizeOptional(mic);
        securityType = NormalizeOptional(securityType);
        currency = NormalizeOptional(currency);

        var parameters = new Dictionary<string, string?>
        {
            ["exchange"] = exchange,
            ["mic"] = mic,
            ["securityType"] = securityType,
            ["currency"] = currency
        };
        return GetFromApiAsync<IReadOnlyList<StockSymbol>>("/stock/symbol", parameters, ct);
    }

    public Task<BasicFinancials> GetBasicFinancialsAsync(string symbol, string metric = "all", CancellationToken ct = default)
    {
        symbol = ValidateRequired(symbol, nameof(symbol));
        metric = ValidateRequired(metric, nameof(metric));

        var parameters = new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
            ["metric"] = metric
        };
        return GetFromApiAsync<BasicFinancials>("/stock/metric", parameters, ct);
    }

    public Task<MarketStatus> GetMarketStatusAsync(string exchange, CancellationToken ct = default)
    {
        exchange = ValidateRequired(exchange, nameof(exchange));
        var parameters = new Dictionary<string, string?> { ["exchange"] = exchange };
        return GetFromApiAsync<MarketStatus>("/stock/market-status", parameters, ct);
    }

    private static string ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
        }

        return value;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static void ValidateAtLeastOneIdentifier(string? symbol, string? isin, string? cusip)
    {
        if (string.IsNullOrWhiteSpace(symbol) &&
            string.IsNullOrWhiteSpace(isin) &&
            string.IsNullOrWhiteSpace(cusip))
        {
            throw new ArgumentException("At least one of 'symbol', 'isin', or 'cusip' must be provided.");
        }
    }

    private static string NormalizePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return path.TrimStart('/');
    }

    private static string NormalizeBaseUrl(string baseUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        return baseUrl.TrimEnd('/') + "/";
    }
}
