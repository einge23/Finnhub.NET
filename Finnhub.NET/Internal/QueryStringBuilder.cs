using System.Text;

namespace Finnhub.NET.Internal;

internal static class QueryStringBuilder
{
    public static string Build(string path, IReadOnlyDictionary<string, string?>? queryParameters)
    {
        if (queryParameters is null || queryParameters.Count == 0)
        {
            return path;
        }

        var builder = new StringBuilder(path);
        var hasAny = false;

        foreach (var (key, value) in queryParameters)
        {
            if (string.IsNullOrWhiteSpace(key) || value is null)
            {
                continue;
            }

            if (!hasAny)
            {
                builder.Append(path.Contains('?', StringComparison.Ordinal) ? '&' : '?');
                hasAny = true;
            }
            else
            {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }

        return builder.ToString();
    }
}
