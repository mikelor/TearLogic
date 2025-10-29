namespace Service.CBInsights.Infrastructure;

/// <summary>
/// Provides reusable Polly policies for HTTP resilience.
/// </summary>
public static class PollyPolicies
{
    /// <summary>
    /// Creates a jittered retry policy for transient failures.
    /// </summary>
    /// <returns>The retry policy.</returns>
    public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), retryCount: 5);
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => (int)response.StatusCode >= 500 || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(delay);
    }

    /// <summary>
    /// Creates a circuit breaker policy for persistent failures.
    /// </summary>
    /// <returns>The circuit breaker policy.</returns>
    public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => (int)response.StatusCode >= 500)
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
    }
}
