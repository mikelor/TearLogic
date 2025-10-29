using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Extensions.Options;
using Service.CBInsights.Configuration;
using TearLogic.Clients;

namespace Service.CBInsights.Services;

/// <summary>
/// Default implementation of <see cref="ICBInsightsClientFactory"/> using Microsoft Kiota.
/// </summary>
/// <param name="httpClientFactory">The HTTP client factory used to create resilient clients.</param>
/// <param name="options">The CB Insights options used to configure the client.</param>
public sealed class CBInsightsClientFactory(IHttpClientFactory httpClientFactory, IOptions<CBInsightsOptions> options) : ICBInsightsClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly CBInsightsOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly IAuthenticationProvider _authenticationProvider = new AnonymousAuthenticationProvider();

    /// <inheritdoc />
    public CBInsightsApiClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient(ICBInsightsClientFactory.HttpClientName);
        var adapter = new HttpClientRequestAdapter(_authenticationProvider, httpClient: httpClient, baseUrl: _options.BaseUrl);
        return new CBInsightsApiClient(adapter);
    }
}
