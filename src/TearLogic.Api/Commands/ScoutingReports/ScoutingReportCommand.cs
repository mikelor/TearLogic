namespace TearLogic.Api.CBInsights.Commands.ScoutingReports;

/// <summary>
/// Represents a command that retrieves a scouting report for a CB Insights organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record ScoutingReportCommand(int OrganizationId);
