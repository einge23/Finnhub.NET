namespace Finnhub.NET.Models;

public sealed record SymbolLookupInfo(
    string Description,
    string DisplaySymbol,
    string Symbol,
    string Type);