using System.Collections.Generic;

namespace Service.CBInsights.Models;

/// <summary>
/// Provides a simplified view of an organization returned from CB Insights.
/// </summary>
public sealed class OrgSummary
{
    /// <summary>
    /// Gets or sets the CB Insights organization identifier.
    /// </summary>
    public int? OrgId { get; set; }

    /// <summary>
    /// Gets or sets the primary organization name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets descriptive information for the organization.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets aliases associated with the organization.
    /// </summary>
    public IReadOnlyCollection<string>? Aliases { get; set; }

    /// <summary>
    /// Gets or sets the URLs associated with the organization.
    /// </summary>
    public IReadOnlyCollection<string>? Urls { get; set; }
}
