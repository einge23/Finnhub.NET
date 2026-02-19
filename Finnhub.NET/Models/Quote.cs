using System.Text.Json.Serialization;

namespace Finnhub.NET.Models;

public sealed record Quote(
    [property: JsonPropertyName("o")]  double OpenPrice,
    [property: JsonPropertyName("h")]  double HighPrice,
    [property: JsonPropertyName("c")]  double CurrentPrice,
    [property: JsonPropertyName("l")]  double LowPrice,
    [property: JsonPropertyName("pc")] double PreviousClosePrice,
    [property: JsonPropertyName("d")]  double Change,
    [property: JsonPropertyName("dp")] double PercentChange);