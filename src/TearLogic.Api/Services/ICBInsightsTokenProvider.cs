namespace Service.CBInsights.Services;

/// <summary>
/// Defines the contract for retrieving CB Insights bearer tokens.
/// </summary>
public interface ICBInsightsTokenProvider
{
    /// <summary>
    /// Retrieves a valid CB Insights bearer token, refreshing the cached token when necessary.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>A task that resolves to a bearer token string.</returns>
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}
