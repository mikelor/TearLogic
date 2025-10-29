using TearLogic.Api.CBInsights.Requests;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for CB Insights organization lookup operations.
/// </summary>
[ApiController]
[Route("api/cbinsights/organizations")]
public sealed class OrganizationLookupController
(
    IOrganizationLookupCommandHandler commandHandler
) : ControllerBase
{
    private readonly IOrganizationLookupCommandHandler _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));

    /// <summary>
    /// Performs an organization lookup using CB Insights.
    /// </summary>
    /// <param name="request">The lookup request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The lookup response.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrgLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] OrganizationLookupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new OrganizationLookupCommand(request.ToKiotaModel());
        var response = await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        return Ok(response);
    }
}

/// <summary>
/// Defines the command handler abstraction for organization lookup.
/// </summary>
public interface IOrganizationLookupCommandHandler : ICommandHandler<OrganizationLookupCommand, OrgLookupResponse?>
{
}

/// <summary>
/// Handles the organization lookup command.
/// </summary>
public sealed class OrganizationLookupCommandHandler
(
    ICBInsightsClient cbInsightsClient,
    ILogger<OrganizationLookupCommandHandler> logger
) : CommandHandler<OrganizationLookupCommand, OrgLookupResponse?>(logger), IOrganizationLookupCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<OrgLookupResponse?> HandleAsync(OrganizationLookupCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var response = await _cbInsightsClient.LookupOrganizationAsync(command.Request, cancellationToken).ConfigureAwait(false);
        return response;
    }
}
