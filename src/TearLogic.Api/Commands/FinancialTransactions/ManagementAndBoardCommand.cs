using TearLogic.Clients.Models.V2ManagementAndBoard;

namespace TearLogic.Api.CBInsights.Commands.FinancialTransactions;

/// <summary>
/// Represents a command to retrieve management and board information for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
/// <param name="Request">The CB Insights request payload.</param>
public sealed record ManagementAndBoardCommand(int OrganizationId, ManagementAndBoardRequestBody Request);
