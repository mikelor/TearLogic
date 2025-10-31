using TearLogic.Clients.Models.V2FinancialTransactions;

namespace TearLogic.Api.CBInsights.Commands.FinancialTransactions;

/// <summary>
/// Represents a command to retrieve investment transactions for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
/// <param name="Request">The CB Insights request payload.</param>
public sealed record InvestmentsCommand(int OrganizationId, ListTransactionsForOrganizationRequest Request);
