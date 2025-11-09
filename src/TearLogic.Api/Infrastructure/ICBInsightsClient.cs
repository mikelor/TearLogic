using TearLogic.Clients.Models.V2BusinessRelationships;
using TearLogic.Clients.Models.V2FinancialTransactions;
using TearLogic.Clients.Models.V2Firmographics;
using TearLogic.Clients.Models.V2ManagementAndBoard;
using TearLogic.Clients.Models.V2OrganizationLookup;
using TearLogic.Clients.Models.V2Outlook;

namespace TearLogic.Api.CBInsights.Infrastructure;

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

    /// <summary>
    /// Retrieves firmographics information for the specified organizations.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The firmographics response.</returns>
    Task<FirmographicsResponse?> GetFirmographicsAsync(FirmographicsRequestBody request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves funding transactions for the specified organization.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The funding transactions response.</returns>
    Task<FundingsResponse?> GetFundingsAsync(int organizationId, ListTransactionsForOrganizationRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves investment transactions for the specified organization.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The investments response.</returns>
    Task<InvestmentsResponse?> GetInvestmentsAsync(int organizationId, ListTransactionsForOrganizationRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves portfolio exit transactions for the specified organization.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The portfolio exits response.</returns>
    Task<PortfolioExitsResponse?> GetPortfolioExitsAsync(int organizationId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves business relationships for the specified organization.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The business relationships response.</returns>
    Task<BusinessRelationshipsResponse?> GetBusinessRelationshipsAsync(int organizationId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves management and board details for the specified organization.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The management and board response.</returns>
    Task<ManagementAndBoardResponse?> GetManagementAndBoardAsync(int organizationId, ManagementAndBoardRequestBody request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves outlook information for the specified organization.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outlook response.</returns>
    Task<OutlookResponse?> GetOutlookAsync(int organizationId, CancellationToken cancellationToken);
}
