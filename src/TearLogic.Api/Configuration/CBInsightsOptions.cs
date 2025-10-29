using System.ComponentModel.DataAnnotations;

namespace Service.CBInsights.Configuration;

/// <summary>
/// Represents the CB Insights configuration required to authenticate and execute requests.
/// </summary>
public sealed class CBInsightsOptions
{
    /// <summary>
    /// Gets the configuration section name used for binding.
    /// </summary>
    public const string SectionName = "CBInsights";

    /// <summary>
    /// Gets or sets the base URL for the CB Insights API.
    /// </summary>
    [Required]
    [Url]
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the CB Insights client identifier.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the CB Insights client secret.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the number of minutes that a retrieved token remains cached.
    /// </summary>
    [Range(1, 120)]
    public int TokenCacheMinutes { get; set; } = 55;
}
