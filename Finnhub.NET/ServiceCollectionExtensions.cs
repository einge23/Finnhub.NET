using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Finnhub.NET;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFinnhub(this IServiceCollection services, Action<FinnhubOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddOptions<FinnhubOptions>().Configure(configureOptions);

        services.AddSingleton<IFinnhubClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FinnhubOptions>>().Value;
            ValidateOptions(options);

            var httpClient = new HttpClient();
            FinnhubClient.ConfigureHttpClient(httpClient, options);

            return new FinnhubClient(httpClient, options);
        });

        return services;
    }

    private static void ValidateOptions(FinnhubOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException("Finnhub ApiKey is required.");
        }
    }
}
