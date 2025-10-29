using Service.CBInsights.Abstractions;
using Service.CBInsights.Commands;
using Service.CBInsights.Models;

namespace Service.CBInsights.Services;

/// <summary>
/// Handles the execution of <see cref="OrgLookupCommand"/> instances.
/// </summary>
/// <param name="organizationService">The service responsible for organization lookups.</param>
public sealed class OrgLookupCommandHandler(ICBInsightsOrganizationService organizationService) : CommandHandler<OrgLookupCommand, OrgLookupResponse>
{
    private readonly ICBInsightsOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));

    /// <inheritdoc />
    public override Task<OrgLookupResponse> HandleAsync(OrgLookupCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return _organizationService.LookupOrganizationsAsync(command.Request, cancellationToken);
    }
}
