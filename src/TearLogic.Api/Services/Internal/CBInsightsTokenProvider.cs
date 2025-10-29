using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TearLogic.Api.Options;
using TearLogic.Api.Services.Abstractions;

namespace TearLogic.Api.Services.Internal;

internal sealed class CBInsightsTokenProvider : ICBInsightsTokenProvider
{
    private const string CacheKey = "CBInsights:Token";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CBInsightsOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CBInsightsTokenProvider> _logger;

    public CBInsightsTokenProvider(
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory,
        IOptions<CBInsightsOptions> options,
        TimeProvider timeProvider,
        ILogger<CBInsightsTokenProvider> logger)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey, out string? token) && !string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        _logger.LogInformation("Requesting new CB Insights access token.");

        var httpClient = _httpClientFactory.CreateClient(TearLogic.Api.HttpClientNames.Authorization);
        var payload = new AuthorizationRequest(_options.ClientId, _options.ClientSecret);

        using var response = await httpClient.PostAsJsonAsync("v2/authorize", payload, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogError("CB Insights authorization failed with status {StatusCode}: {Body}", (int)response.StatusCode, error);
            throw new HttpRequestException($"CB Insights authorization failed with status code {(int)response.StatusCode}.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var authorizationResponse = await JsonSerializer.DeserializeAsync<AuthorizationResponse>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("CB Insights authorization response was empty.");

        if (string.IsNullOrWhiteSpace(authorizationResponse.Token))
        {
            throw new InvalidOperationException("CB Insights authorization response did not include a token.");
        }

        token = authorizationResponse.Token;

        var expiration = _timeProvider.GetUtcNow().Add(_options.TokenCacheDuration);
        _cache.Set(CacheKey, token, expiration);

        _logger.LogInformation("Obtained CB Insights token. Cached until {Expiration}.", expiration);

        return token;
    }

    private sealed record AuthorizationRequest(string ClientId, string ClientSecret);

    private sealed record AuthorizationResponse(string Token);
}
