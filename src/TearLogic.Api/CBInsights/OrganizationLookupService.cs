using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.CBInsights;

public interface IOrganizationLookupService
{
    Task<OrgLookupResponse?> LookupAsync(OrganizationLookupRequest request, CancellationToken cancellationToken);
}

internal sealed class OrganizationLookupService : IOrganizationLookupService
{
    private readonly ICBInsightsClientFactory _clientFactory;
    private readonly ILogger<OrganizationLookupService> _logger;

    public OrganizationLookupService(ICBInsightsClientFactory clientFactory, ILogger<OrganizationLookupService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<OrgLookupResponse?> LookupAsync(OrganizationLookupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var client = _clientFactory.CreateClient();
            var body = request.ToLookupRequestBody();
            return await client.V2.Organizations.PostAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Microsoft.Kiota.Abstractions.Http.HttpResponseException ex)
        {
            _logger.LogError(ex, "CB Insights responded with status code {StatusCode}", (int)ex.ResponseStatusCode);
            throw new CBInsightsRequestException((System.Net.HttpStatusCode)ex.ResponseStatusCode, "CB Insights returned an error response.");
        }
    }
}
