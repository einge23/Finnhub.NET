using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finnhub.NET.Models;

public sealed record BasicFinancials(
    string Symbol,
    string MetricType,
    [property: JsonPropertyName("metric")] Dictionary<string, JsonElement> Metric,
    [property: JsonPropertyName("series")] Dictionary<string, JsonElement> Series);

