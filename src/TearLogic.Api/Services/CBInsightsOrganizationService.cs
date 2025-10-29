using System.Linq;
using System.Resources;
using Microsoft.Extensions.Logging;
using Service.CBInsights.Models;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace Service.CBInsights.Services;

/// <summary>
/// Implements organization lookup operations against the CB Insights API.
/// </summary>
/// <param name="clientFactory">The factory used to create CB Insights clients.</param>
/// <param name="tokenProvider">The provider responsible for issuing bearer tokens.</param>
/// <param name="logger">The logger used for structured telemetry.</param>
public sealed class CBInsightsOrganizationService(
    ICBInsightsClientFactory clientFactory,
    ICBInsightsTokenProvider tokenProvider,
    ILogger<CBInsightsOrganizationService> logger) : ICBInsightsOrganizationService
{
    private static readonly ResourceManager LogResourceManager = new("Service.CBInsights.Resources.LogMessages", typeof(CBInsightsOrganizationService).Assembly);
    private static readonly ResourceManager ErrorResourceManager = new("Service.CBInsights.Resources.ErrorMessages", typeof(CBInsightsOrganizationService).Assembly);

    private readonly ICBInsightsClientFactory _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    private readonly ICBInsightsTokenProvider _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    private readonly ILogger<CBInsightsOrganizationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<OrgLookupResponse> LookupOrganizationsAsync(OrgLookupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var client = _clientFactory.CreateClient();
        var token = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);

        var requestBody = new OrgLookupRequestBody
        {
            Limit = request.Limit,
            Names = request.Names?.Where(static name => !string.IsNullOrWhiteSpace(name)).Select(static name => name.Trim()).ToList(),
            Urls = request.Urls?.Where(static url => !string.IsNullOrWhiteSpace(url)).Select(static url => url.Trim()).ToList(),
            NextPageToken = request.NextPageToken,
            ProfileUrl = request.ProfileUrl,
            Sort = request.Sort is null ? null : new OrgLookupRequestBody_sort
            {
                Direction = request.Sort.Direction,
                Field = request.Sort.Field
            }
        };

        _logger.LogInformation(LogResourceManager.GetString("OrgLookupRequest"));
        var response = await client.V2.Organizations.PostAsync(
            requestBody,
            requestConfiguration =>
            {
                requestConfiguration.Headers.Add("Authorization", $"Bearer {token}");
            },
            cancellationToken).ConfigureAwait(false);

        if (response is null)
        {
            var message = ErrorResourceManager.GetString("OrgLookupFailed") ?? "Organization lookup failed.";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        _logger.LogInformation(LogResourceManager.GetString("OrgLookupResponse"));

        return new OrgLookupResponse
        {
            NextPageToken = response.NextPageToken,
            TotalHits = response.TotalHits,
            TotalHitsRelation = response.TotalHitsRelation,
            Organizations = response.Orgs?.Select(static org => new OrgSummary
            {
                OrgId = org.OrgId,
                Name = org.Name,
                Description = org.Description,
                Aliases = org.Aliases is null ? null : org.Aliases.ToArray(),
                Urls = org.Urls is null ? null : org.Urls.ToArray()
            }).ToArray() ?? Array.Empty<OrgSummary>()
        };
    }
}
