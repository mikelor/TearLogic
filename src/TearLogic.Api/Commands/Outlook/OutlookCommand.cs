namespace TearLogic.Api.CBInsights.Commands.Outlook;

/// <summary>
/// Represents a command to retrieve outlook information for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record OutlookCommand(int OrganizationId);
