using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.CBInsights.Models;

/// <summary>
/// Represents the request payload accepted by the organization lookup endpoint.
/// </summary>
public sealed class OrgLookupRequest : IValidatableObject
{
    /// <summary>
    /// Gets or sets the maximum number of organizations to return.
    /// </summary>
    [Range(1, 100)]
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the organization names used for lookup.
    /// </summary>
    public IReadOnlyCollection<string>? Names { get; set; }

    /// <summary>
    /// Gets or sets the organization URLs used for lookup.
    /// </summary>
    public IReadOnlyCollection<string>? Urls { get; set; }

    /// <summary>
    /// Gets or sets the token for retrieving subsequent result pages.
    /// </summary>
    public string? NextPageToken { get; set; }

    /// <summary>
    /// Gets or sets the direct CB Insights profile URL for lookup.
    /// </summary>
    [Url]
    public string? ProfileUrl { get; set; }

    /// <summary>
    /// Gets or sets the optional sort configuration.
    /// </summary>
    public OrgLookupSortOptions? Sort { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasNames = Names is { Count: > 0 };
        var hasUrls = Urls is { Count: > 0 };
        var hasProfile = !string.IsNullOrWhiteSpace(ProfileUrl);
        var hasToken = !string.IsNullOrWhiteSpace(NextPageToken);

        if (!hasNames && !hasUrls && !hasProfile && !hasToken)
        {
            yield return new ValidationResult(
                "At least one search parameter must be provided.",
                new[] { nameof(Names), nameof(Urls), nameof(ProfileUrl), nameof(NextPageToken) });
        }

        if (hasProfile && (hasNames || hasUrls))
        {
            yield return new ValidationResult(
                "Profile lookups cannot include name or URL filters.",
                new[] { nameof(ProfileUrl), nameof(Names), nameof(Urls) });
        }
    }
}
