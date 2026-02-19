namespace Finnhub.NET.Models;

public sealed record SymbolSearchLookup(
    SymbolLookupInfo Result,
    long Count);