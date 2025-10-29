using System.Text.Json.Serialization;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api;

public sealed class OrganizationLookupRequest
{
    [JsonPropertyName("names")]
    public IReadOnlyCollection<string>? Names { get; init; }

    [JsonPropertyName("urls")]
    public IReadOnlyCollection<string>? Urls { get; init; }

    [JsonPropertyName("profileUrl")]
    public string? ProfileUrl { get; init; }

    [JsonPropertyName("limit")]
    public int? Limit { get; init; }

    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; init; }

    public bool HasSearchCriteria =>
        (Names is { Count: > 0 }) ||
        (Urls is { Count: > 0 }) ||
        !string.IsNullOrWhiteSpace(ProfileUrl);
}

internal static class OrganizationLookupRequestExtensions
{
    public static OrgLookupRequestBody ToLookupRequestBody(this OrganizationLookupRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new OrgLookupRequestBody
        {
            Names = request.Names?.ToList(),
            Urls = request.Urls?.ToList(),
            ProfileUrl = request.ProfileUrl,
            Limit = request.Limit,
            NextPageToken = request.NextPageToken
        };
    }
}
