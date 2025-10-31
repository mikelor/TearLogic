namespace TearLogic.Api.CBInsights.Commands.FinancialTransactions;

/// <summary>
/// Represents a command to retrieve business relationships for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record BusinessRelationshipsCommand(int OrganizationId);
