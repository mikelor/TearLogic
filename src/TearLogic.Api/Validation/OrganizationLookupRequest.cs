using System.ComponentModel.DataAnnotations;
using System.Linq;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.Validation;

public sealed class OrganizationLookupRequest
{
    public IReadOnlyList<string>? Names { get; init; }

    public IReadOnlyList<string>? Urls { get; init; }

    public int? Limit { get; init; }

    public string? NextPageToken { get; init; }

    public string? ProfileUrl { get; init; }

    public OrganizationLookupRequestSort? Sort { get; init; }

    public bool HasLookupCriteria =>
        (Names is { Count: > 0 }) ||
        (Urls is { Count: > 0 }) ||
        !string.IsNullOrWhiteSpace(ProfileUrl) ||
        !string.IsNullOrWhiteSpace(NextPageToken);

    public OrgLookupRequestBody ToRequestBody()
    {
        var body = new OrgLookupRequestBody
        {
            Limit = Limit,
            Names = Names?.ToList(),
            Urls = Urls?.ToList(),
            NextPageToken = NextPageToken,
            ProfileUrl = ProfileUrl,
            Sort = Sort is null ? null : new OrgLookupRequestBody_sort
            {
                Direction = Sort.Direction,
                Field = Sort.Field
            }
        };

        return body;
    }
}

public sealed class OrganizationLookupRequestSort
{
    [Required]
    [RegularExpression("^(?i)(orgName)$", ErrorMessage = "Sort field must be orgName.")]
    public string? Field { get; init; }

    [Required]
    [RegularExpression("^(?i)(asc|desc)$", ErrorMessage = "Sort direction must be asc or desc.")]
    public string? Direction { get; init; }
}
