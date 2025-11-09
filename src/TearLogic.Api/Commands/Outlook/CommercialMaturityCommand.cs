namespace TearLogic.Api.CBInsights.Commands.Outlook;

/// <summary>
/// Represents a command to retrieve the commercial maturity for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record CommercialMaturityCommand(int OrganizationId);
