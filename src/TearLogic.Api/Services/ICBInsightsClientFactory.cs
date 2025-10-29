using TearLogic.Clients;

namespace Service.CBInsights.Services;

/// <summary>
/// Provides a factory for creating configured CB Insights API clients.
/// </summary>
public interface ICBInsightsClientFactory
{
    /// <summary>
    /// Gets the named HTTP client registration used for downstream CB Insights calls.
    /// </summary>
    public const string HttpClientName = "CBInsights";

    /// <summary>
    /// Creates a new instance of the CB Insights API client.
    /// </summary>
    /// <returns>A configured <see cref="CBInsightsApiClient"/> instance.</returns>
    CBInsightsApiClient CreateClient();
}
