# Finnhub.NET

Lightweight .NET client for the [Finnhub API](https://finnhub.io/docs/api).

## Install

```bash
dotnet add package Finnhub.NET
```

## Quick Start

```csharp
using Finnhub.NET;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddFinnhub(options =>
{
    options.ApiKey = "<FINNHUB_API_KEY>";
});

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IFinnhubClient>();

var quote = await client.GetQuoteAsync("AAPL");
Console.WriteLine($"Current: {quote.CurrentPrice}");
```

## Notes

- Requires a Finnhub API key.
- Supports core stock endpoints like quote, profile, candles, news, peers, symbols, metrics, and market status.
