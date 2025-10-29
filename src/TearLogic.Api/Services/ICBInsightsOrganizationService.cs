using Service.CBInsights.Models;

namespace Service.CBInsights.Services;

/// <summary>
/// Defines operations for retrieving organization data from CB Insights.
/// </summary>
public interface ICBInsightsOrganizationService
{
    /// <summary>
    /// Executes the organization lookup request.
    /// </summary>
    /// <param name="request">The lookup request to execute.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>A task that resolves to the lookup response.</returns>
    Task<OrgLookupResponse> LookupOrganizationsAsync(OrgLookupRequest request, CancellationToken cancellationToken);
}
