using System.ComponentModel.DataAnnotations;

namespace TearLogic.Api.Options;

public sealed class CBInsightsOptions
{
    public const string SectionName = "CBInsights";

    [Required]
    [Url]
    public string BaseUrl { get; set; } = "https://api.cbinsights.com/";

    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration to cache authorization tokens before requesting a new one.
    /// Defaults to 55 minutes to stay within the expected one-hour expiry window.
    /// </summary>
    public TimeSpan TokenCacheDuration { get; set; } = TimeSpan.FromMinutes(55);
}
