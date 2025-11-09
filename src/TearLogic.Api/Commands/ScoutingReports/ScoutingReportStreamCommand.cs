namespace TearLogic.Api.CBInsights.Commands.ScoutingReports;

/// <summary>
/// Represents a command that streams a scouting report for a CB Insights organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record ScoutingReportStreamCommand(int OrganizationId);
