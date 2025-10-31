using System.ComponentModel.DataAnnotations;
using TearLogic.Clients.Models.V2FinancialTransactions;

namespace TearLogic.Api.CBInsights.Requests;

/// <summary>
/// Represents a query for paginated financial transactions for an organization.
/// </summary>
public sealed class FinancialTransactionsListRequest
{
    /// <summary>
    /// Gets or sets the maximum number of transactions to retrieve.
    /// </summary>
    [Range(1, 100)]
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the pagination token supplied by a previous response.
    /// </summary>
    public string? NextPageToken { get; set; }

    /// <summary>
    /// Converts the request to the Kiota model used by the CB Insights client.
    /// </summary>
    /// <returns>The populated Kiota request model.</returns>
    public ListTransactionsForOrganizationRequest ToKiotaModel()
    {
        return new ListTransactionsForOrganizationRequest
        {
            Limit = Limit,
            NextPageToken = string.IsNullOrWhiteSpace(NextPageToken) ? null : NextPageToken
        };
    }
}
