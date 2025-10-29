using System;
using System.Collections.Generic;

namespace Service.CBInsights.Models;

/// <summary>
/// Represents the API response returned to the Glean action.
/// </summary>
public sealed class OrgLookupResponse
{
    /// <summary>
    /// Gets or sets the token used to fetch subsequent result pages.
    /// </summary>
    public string? NextPageToken { get; set; }

    /// <summary>
    /// Gets or sets the total number of organizations matching the search criteria.
    /// </summary>
    public int? TotalHits { get; set; }

    /// <summary>
    /// Gets or sets the relation of the total hits to the result set size.
    /// </summary>
    public string? TotalHitsRelation { get; set; }

    /// <summary>
    /// Gets or sets the organizations returned by the lookup operation.
    /// </summary>
    public IReadOnlyCollection<OrgSummary> Organizations { get; set; } = Array.Empty<OrgSummary>();
}
