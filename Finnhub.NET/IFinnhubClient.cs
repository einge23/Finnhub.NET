using Finnhub.NET.Models;

namespace Finnhub.NET;

public interface IFinnhubClient
{
    // /quote
    Task<Quote> GetQuoteAsync(string symbol, CancellationToken ct = default);
    
    // /stock/profile
    Task<CompanyProfile> GetCompanyProfileAsync(string? symbol = null, string? isin = null, string? cusip = null,
        CancellationToken ct = default);
    
    // /stock/profile2
    Task<CompanyProfile2> GetCompanyProfile2Async(string? symbol = null, string? isin = null, string? cusip = null,
        CancellationToken ct = default);

    // /stock/candle
    Task<StockCandles> GetStockCandlesAsync(string symbol, string resolution, DateTimeOffset from, DateTimeOffset to,
        CancellationToken ct = default);
    
    // /company-news
    Task<IReadOnlyList<CompanyNews>> GetCompanyNewsAsync(string symbol, DateOnly from, DateOnly to,
        CancellationToken ct = default);
    
    // /stock/peers
    Task<IReadOnlyList<string>> GetStockPeersAsync(string symbol, string? grouping, CancellationToken ct = default);

    // /stock/symbol
    Task<IReadOnlyList<StockSymbol>> GetStockSymbolsAsync(string exchange, string? mic, string? securityType,
        string? currency, CancellationToken ct = default);
    
    // /stock/metric
    Task<BasicFinancials> GetBasicFinancialsAsync(string symbol, string metric = "all", CancellationToken ct = default);
    
    // /stock/market-status
    Task<MarketStatus> GetMarketStatusAsync(string exchange, CancellationToken ct = default);



}
