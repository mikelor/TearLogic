using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.CBInsights.Commands.OrganizationLookup;

/// <summary>
/// Represents an organization lookup command.
/// </summary>
/// <param name="Request">The CB Insights request body.</param>
public sealed record OrganizationLookupCommand(OrgLookupRequestBody Request);
