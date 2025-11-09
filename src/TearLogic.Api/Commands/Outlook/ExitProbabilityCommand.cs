namespace TearLogic.Api.CBInsights.Commands.Outlook;

/// <summary>
/// Represents a command to retrieve exit probability information for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record ExitProbabilityCommand(int OrganizationId);
