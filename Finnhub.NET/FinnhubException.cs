namespace Finnhub.NET;

public class FinnhubException : Exception
{
    public int StatusCode { get; }

    public string ResponseBody { get; }

    public FinnhubException(string message, int statusCode, string responseBody)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public FinnhubException(string message, int statusCode, string responseBody, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
