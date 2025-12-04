namespace TearLogic.Api.CBInsights.Infrastructure;

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
            _logger.LogTokenCacheHit();
            return token;
        }

        _logger.LogTokenRefreshing();

        var options = _optionsMonitor.CurrentValue;
        var client = _httpClientFactory.CreateClient(CBInsightsHttpClientNames.Authorization);
        var payload = new AuthorizationRequest(options.ClientId, options.ClientSecret);
        using var response = await client.PostAsJsonAsync(options.AuthorizeEndpoint, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogTokenAcquisitionFailed(response.StatusCode);
            throw new InvalidOperationException("Unable to acquire a CB Insights authorization token.");
        }

        var authorization = await response.Content.ReadFromJsonAsync<AuthorizationResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (authorization?.Token is null)
        {
            _logger.LogTokenResponseInvalid();
            throw new InvalidOperationException("The CB Insights authorization response did not include a token.");
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
