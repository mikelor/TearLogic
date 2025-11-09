namespace TearLogic.Api.CBInsights.Commands.Outlook;

/// <summary>
/// Represents a command to retrieve Mosaic score information for an organization.
/// </summary>
/// <param name="OrganizationId">The CB Insights organization identifier.</param>
public sealed record MosaicScoreCommand(int OrganizationId);
