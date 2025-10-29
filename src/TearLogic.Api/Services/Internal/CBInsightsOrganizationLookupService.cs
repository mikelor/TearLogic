using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using TearLogic.Api.Options;
using TearLogic.Api.Services.Abstractions;
using TearLogic.Api.Validation;
using TearLogic.Clients;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.Services.Internal;

internal sealed class CBInsightsOrganizationLookupService : ICBInsightsOrganizationLookupService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly CBInsightsOptions _options;

    public CBInsightsOrganizationLookupService(
        IHttpClientFactory httpClientFactory,
        IAuthenticationProvider authenticationProvider,
        IOptions<CBInsightsOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _authenticationProvider = authenticationProvider;
        _options = options.Value;
    }

    public async Task<OrgLookupResponse?> LookupOrganizationsAsync(OrganizationLookupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var httpClient = _httpClientFactory.CreateClient(TearLogic.Api.HttpClientNames.Api);
        var adapter = new HttpClientRequestAdapter(_authenticationProvider, httpClient)
        {
            BaseUrl = _options.BaseUrl
        };

        var client = new CBInsightsApiClient(adapter);
        return await client.V2.Organizations.PostAsync(request.ToRequestBody(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
