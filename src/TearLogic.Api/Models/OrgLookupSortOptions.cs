using System.ComponentModel.DataAnnotations;

namespace Service.CBInsights.Models;

/// <summary>
/// Represents optional sorting metadata for organization lookups.
/// </summary>
public sealed class OrgLookupSortOptions
{
    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "Sort direction must be 'asc' or 'desc'.")]
    public string? Direction { get; set; }

    /// <summary>
    /// Gets or sets the field used for sorting.
    /// </summary>
    [RegularExpression("^orgName$", ErrorMessage = "Only 'orgName' is supported as a sort field.")]
    public string? Field { get; set; }
}
