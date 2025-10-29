using System.ComponentModel.DataAnnotations;

namespace TearLogic.Api.CBInsights;

public sealed class CBInsightsOptions
{
    public const string SectionName = "CBInsights";

    [Required]
    public string ClientId { get; init; } = string.Empty;

    [Required]
    public string ClientSecret { get; init; } = string.Empty;
}
