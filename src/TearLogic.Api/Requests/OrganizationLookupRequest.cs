using System.ComponentModel.DataAnnotations;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace Service.CBInsights.Requests;

/// <summary>
/// Represents an API request for CB Insights organization lookup.
/// </summary>
public sealed class OrganizationLookupRequest
{
    /// <summary>
    /// Gets or sets the names to match.
    /// </summary>
    public ICollection<string>? Names { get; set; }

    /// <summary>
    /// Gets or sets the URLs to match.
    /// </summary>
    public ICollection<string>? Urls { get; set; }

    /// <summary>
    /// Gets or sets the profile URL.
    /// </summary>
    [Url]
    public string? ProfileUrl { get; set; }

    /// <summary>
    /// Gets or sets the pagination token.
    /// </summary>
    public string? NextPageToken { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return.
    /// </summary>
    [Range(1, 100)]
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the sort options.
    /// </summary>
    public OrganizationLookupRequestSort? Sort { get; set; }

    /// <summary>
    /// Converts the request into a Kiota request body.
    /// </summary>
    /// <returns>The Kiota request body.</returns>
    public OrgLookupRequestBody ToKiotaModel()
    {
        return new OrgLookupRequestBody
        {
            Names = Names?.ToList(),
            Urls = Urls?.ToList(),
            ProfileUrl = ProfileUrl,
            NextPageToken = NextPageToken,
            Limit = Limit,
            Sort = Sort?.ToKiotaModel()
        };
    }
}

/// <summary>
/// Represents sort options for organization lookup.
/// </summary>
public sealed class OrganizationLookupRequestSort
{
    /// <summary>
    /// Gets or sets the sort field.
    /// </summary>
    [Required]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    [Required]
    public string? Direction { get; set; }

    /// <summary>
    /// Converts the request into a Kiota sort model.
    /// </summary>
    /// <returns>The Kiota model.</returns>
    public OrgLookupRequestBody_sort ToKiotaModel()
    {
        return new OrgLookupRequestBody_sort
        {
            Field = Field,
            Direction = Direction
        };
    }
}
