using System.ComponentModel.DataAnnotations;

namespace Finnhub.NET;

public sealed class FinnhubOptions
{
    public const string SectionName = "Finnhub";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://finnhub.io/api/v1";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
