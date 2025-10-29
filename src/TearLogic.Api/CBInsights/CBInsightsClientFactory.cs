using Microsoft.Kiota.Http.HttpClientLibrary;
using TearLogic.Clients;

namespace TearLogic.Api.CBInsights;

public interface ICBInsightsClientFactory
{
    CBInsightsApiClient CreateClient();
}

internal sealed class CBInsightsClientFactory : ICBInsightsClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CBInsightsAuthenticationProvider _authenticationProvider;

    public CBInsightsClientFactory(IHttpClientFactory httpClientFactory, CBInsightsAuthenticationProvider authenticationProvider)
    {
        _httpClientFactory = httpClientFactory;
        _authenticationProvider = authenticationProvider;
    }

    public CBInsightsApiClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient(CBInsightsHttpClientNames.Api);
        var adapter = new HttpClientRequestAdapter(_authenticationProvider, httpClient: httpClient)
        {
            BaseUrl = "https://api.cbinsights.com"
        };

        return new CBInsightsApiClient(adapter);
    }
}
