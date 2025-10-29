using Service.CBInsights.Models;

namespace Service.CBInsights.Commands;

/// <summary>
/// Represents the command payload for initiating an organization lookup operation.
/// </summary>
/// <param name="Request">The validated lookup request details.</param>
public sealed record OrgLookupCommand(OrgLookupRequest Request);
