namespace Finnhub.NET.Models;

public sealed record CompanyProfile2(
    string Country,
    string Currency,
    string Exchange,
    string Name,
    string Ticker,
    DateOnly Ipo,
    double MarketCapitalization,
    double ShareOutstanding,
    string Logo,
    string Phone,
    string WebUrl,
    string FinnhubIndustry);
