using TearLogic.Clients.Models.V2OrganizationLookup;
using TearLogic.Api.Validation;

namespace TearLogic.Api.Services.Abstractions;

public interface ICBInsightsOrganizationLookupService
{
    Task<OrgLookupResponse?> LookupOrganizationsAsync(OrganizationLookupRequest request, CancellationToken cancellationToken);
}
