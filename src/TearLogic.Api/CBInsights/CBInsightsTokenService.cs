using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TearLogic.Api.CBInsights;

public interface ICBInsightsTokenService
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}

internal sealed class CBInsightsTokenService : ICBInsightsTokenService
{
    private const string CacheKey = "CBInsights.Token";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(55);
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly IOptions<CBInsightsOptions> _options;
    private readonly ILogger<CBInsightsTokenService> _logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    public CBInsightsTokenService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache,
        IOptions<CBInsightsOptions> options,
        ILogger<CBInsightsTokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _options = options;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(CacheKey, out string? token) && !string.IsNullOrEmpty(token))
        {
            return token;
        }

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_memoryCache.TryGetValue(CacheKey, out token) && !string.IsNullOrEmpty(token))
            {
                return token;
            }

            var client = _httpClientFactory.CreateClient(CBInsightsHttpClientNames.Auth);
            var payload = new AuthorizeRequest(_options.Value.ClientId, _options.Value.ClientSecret);
            using var response = await client.PostAsJsonAsync("/v2/authorize", payload, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("CB Insights authorization failed with status code {StatusCode}: {Error}", (int)response.StatusCode, error);
                throw new CBInsightsAuthenticationException($"CB Insights authorization failed with status code {(int)response.StatusCode}.");
            }

            var authorizeResponse = await response.Content.ReadFromJsonAsync<AuthorizeResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            token = authorizeResponse?.Token;

            if (string.IsNullOrEmpty(token))
            {
                throw new CBInsightsAuthenticationException("CB Insights authorization response did not include a token.");
            }

            _memoryCache.Set(CacheKey, token, CacheDuration);
            _logger.LogInformation("Retrieved new CB Insights token.");

            return token;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex) when (ex is not CBInsightsAuthenticationException)
        {
            throw new CBInsightsAuthenticationException("Failed to retrieve CB Insights token.", ex);
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private sealed record AuthorizeRequest(string ClientId, string ClientSecret);

    private sealed record AuthorizeResponse(string Token);
}
