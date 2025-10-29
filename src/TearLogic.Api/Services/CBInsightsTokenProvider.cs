using System.Resources;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.CBInsights.Configuration;
using TearLogic.Clients.Models.V2Authorize;

namespace Service.CBInsights.Services;

/// <summary>
/// Provides token acquisition and caching for CB Insights interactions.
/// </summary>
/// <param name="memoryCache">The cache used to store the token.</param>
/// <param name="clientFactory">The factory responsible for creating CB Insights clients.</param>
/// <param name="options">The configuration options controlling authentication behavior.</param>
/// <param name="logger">The logger used for telemetry.</param>
public sealed class CBInsightsTokenProvider(
    IMemoryCache memoryCache,
    ICBInsightsClientFactory clientFactory,
    IOptions<CBInsightsOptions> options,
    ILogger<CBInsightsTokenProvider> logger) : ICBInsightsTokenProvider
{
    private static readonly ResourceManager LogResourceManager = new("Service.CBInsights.Resources.LogMessages", typeof(CBInsightsTokenProvider).Assembly);
    private static readonly ResourceManager ErrorResourceManager = new("Service.CBInsights.Resources.ErrorMessages", typeof(CBInsightsTokenProvider).Assembly);
    private const string TokenCacheKey = "CBInsights::BearerToken";

    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ICBInsightsClientFactory _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    private readonly CBInsightsOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<CBInsightsTokenProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
        {
            _logger.LogInformation(LogResourceManager.GetString("TokenCacheHit"));
            return cachedToken;
        }

        _logger.LogInformation(LogResourceManager.GetString("TokenCacheMiss"));
        var requestBody = new AuthorizeRequestBody
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret
        };

        var client = _clientFactory.CreateClient();
        _logger.LogInformation(LogResourceManager.GetString("AuthorizeRequest"));
        var response = await client.V2.Authorize.PostAsync(requestBody, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (response?.Token is not { Length: > 0 } token)
        {
            var message = ErrorResourceManager.GetString("TokenAcquisitionFailed") ?? "Token acquisition failed.";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        _logger.LogInformation(LogResourceManager.GetString("AuthorizeResponse"));
        _memoryCache.Set(TokenCacheKey, token, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.TokenCacheMinutes)
        });

        return token;
    }
}
