using System.Net;

namespace TearLogic.Api.CBInsights;

public sealed class CBInsightsAuthenticationException : Exception
{
    public CBInsightsAuthenticationException(string message) : base(message)
    {
    }

    public CBInsightsAuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public sealed class CBInsightsRequestException : Exception
{
    public CBInsightsRequestException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
