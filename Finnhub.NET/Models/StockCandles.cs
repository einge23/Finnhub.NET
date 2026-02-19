using System.Text.Json.Serialization;

namespace Finnhub.NET.Models;

public sealed record StockCandles(
    [property: JsonPropertyName("c")] decimal[] ClosePrices,
    [property: JsonPropertyName("h")] decimal[] HighPrices,
    [property: JsonPropertyName("l")] decimal[] LowPrices,
    [property: JsonPropertyName("o")] decimal[] OpenPrices,
    [property: JsonPropertyName("v")] decimal[] Volumes,
    [property: JsonPropertyName("t")] long[] Timestamps,
    [property: JsonPropertyName("s")] string Status);