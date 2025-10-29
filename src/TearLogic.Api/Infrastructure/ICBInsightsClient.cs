using TearLogic.Clients.Models.V2OrganizationLookup;

namespace Service.CBInsights.Infrastructure;

/// <summary>
/// Provides CB Insights client operations.
/// </summary>
public interface ICBInsightsClient
{
    /// <summary>
    /// Executes an organization lookup using the CB Insights API.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The lookup response.</returns>
    Task<OrgLookupResponse?> LookupOrganizationAsync(OrgLookupRequestBody request, CancellationToken cancellationToken);
}
