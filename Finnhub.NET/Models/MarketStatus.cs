namespace Finnhub.NET.Models;

public sealed record MarketStatus(
    string Exchange,
    string Timezone,
    string Session,
    string Holiday,
    bool IsOpen,
    long T);

