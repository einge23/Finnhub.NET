namespace Finnhub.NET.Models;

public sealed record StockSymbol(
    string Description,
    string DisplaySymbol,
    string Symbol,
    string Type,
    string Mic,
    string Figi,
    string ShareClassFigi,
    string Currency,
    string Symbol2,
    string Isin);
