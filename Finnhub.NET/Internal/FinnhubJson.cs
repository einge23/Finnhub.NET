using System.Text.Json;

namespace Finnhub.NET.Internal;

internal static class FinnhubJson
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}
