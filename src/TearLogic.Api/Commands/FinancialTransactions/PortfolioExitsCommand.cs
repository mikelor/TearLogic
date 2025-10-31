namespace TearLogic.Api.CBInsights.Commands.FinancialTransactions;

/// <summary>
/// Represents a command to retrieve portfolio exit transactions for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record PortfolioExitsCommand(int OrganizationId);
