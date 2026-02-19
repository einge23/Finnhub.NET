namespace Finnhub.NET.Models;

public sealed record CompanyNews(
    string Category,
    long DateTime,
    string Headline,
    long Id,
    string Image,
    string Related,
    string Source,
    string Summary,
    string Url);