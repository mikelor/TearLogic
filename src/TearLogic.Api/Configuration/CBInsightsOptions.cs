using System.ComponentModel.DataAnnotations;

namespace TearLogic.Api.CBInsights.Configuration;

/// <summary>
/// Represents CB Insights configuration settings.
/// </summary>
public sealed class CBInsightsOptions
{
    /// <summary>
    /// Gets or sets the CB Insights API base URL.
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authorization endpoint path.
    /// </summary>
    [Required]
    public string AuthorizeEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organizations endpoint path.
    /// </summary>
    [Required]
    public string OrganizationsEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CB Insights client identifier.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CB Insights client secret.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cache duration in minutes for authorization tokens.
    /// </summary>
    [Range(1, 240)]
    public int TokenCacheDurationMinutes { get; set; } = 55;
}
