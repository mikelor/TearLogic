using System.Net.Http.Json;

namespace Service.CBInsights.Infrastructure;

/// <summary>
/// Provides tokens for authenticating CB Insights requests.
/// </summary>
public interface ICBInsightsTokenProvider
{
    /// <summary>
    /// Retrieves a valid authorization token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bearer token string.</returns>
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Default implementation of <see cref="ICBInsightsTokenProvider"/>.
/// </summary>
public sealed class CBInsightsTokenProvider
(
    IMemoryCache memoryCache,
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<CBInsightsOptions> optionsMonitor,
    ILogger<CBInsightsTokenProvider> logger,
    ILogMessageProvider logMessageProvider,
    IErrorMessageProvider errorMessageProvider
) : ICBInsightsTokenProvider
{
    private const string CacheKey = nameof(CBInsightsTokenProvider) + ":Token";
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly IOptionsMonitor<CBInsightsOptions> _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
    private readonly ILogger<CBInsightsTokenProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ILogMessageProvider _logMessageProvider = logMessageProvider ?? throw new ArgumentNullException(nameof(logMessageProvider));
    private readonly IErrorMessageProvider _errorMessageProvider = errorMessageProvider ?? throw new ArgumentNullException(nameof(errorMessageProvider));

    /// <inheritdoc />
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(CacheKey, out string? token) && !string.IsNullOrWhiteSpace(token))
        {
            var cacheMessage = _logMessageProvider.GetString("TokenCacheHit");
            if (!string.IsNullOrWhiteSpace(cacheMessage))
            {
                _logger.LogInformation(cacheMessage);
            }

            return token;
        }

        var refreshMessage = _logMessageProvider.GetString("TokenRefreshing");
        if (!string.IsNullOrWhiteSpace(refreshMessage))
        {
            _logger.LogInformation(refreshMessage);
        }

        var options = _optionsMonitor.CurrentValue;
        var client = _httpClientFactory.CreateClient(CBInsightsHttpClientNames.Authorization);
        var payload = new AuthorizationRequest(options.ClientId, options.ClientSecret);
        using var response = await client.PostAsJsonAsync(options.AuthorizeEndpoint, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = _errorMessageProvider.GetString("TokenAcquisitionFailed") ?? "Unable to acquire a CB Insights authorization token.";
            _logger.LogError("{Message} StatusCode: {StatusCode}", errorMessage, response.StatusCode);
            throw new InvalidOperationException(errorMessage);
        }

        var authorization = await response.Content.ReadFromJsonAsync<AuthorizationResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (authorization?.Token is null)
        {
            var errorMessage = _errorMessageProvider.GetString("TokenResponseInvalid") ?? "The CB Insights authorization response did not include a token.";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.TokenCacheDurationMinutes)
        };

        _memoryCache.Set(CacheKey, authorization.Token, cacheEntryOptions);
        return authorization.Token;
    }

    private sealed record AuthorizationRequest(string ClientId, string ClientSecret);

    private sealed record AuthorizationResponse(string Token);
}
