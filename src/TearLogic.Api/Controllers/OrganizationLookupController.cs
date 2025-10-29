using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// Retrieves a list of organizations matching the supplied query parameters.
    /// </summary>
    /// <param name="request">The lookup request constructed from query parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The lookup response.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(OrgLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync([FromQuery] OrganizationLookupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!HasLookupCriteria(request))
        {
            ModelState.AddModelError(nameof(request), "At least one search parameter must be provided.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new OrganizationLookupCommand(request.ToKiotaModel());
        var response = await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a single organization by name.
    /// </summary>
    /// <param name="organization">The organization name to look up.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching organization if found.</returns>
    [HttpGet("{organization}")]
    [ProducesResponseType(typeof(Org), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByOrganizationAsync([FromRoute] string organization, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organization))
        {
            ModelState.AddModelError(nameof(organization), "An organization name is required.");
            return ValidationProblem(ModelState);
        }

        var lookupRequest = new OrganizationLookupRequest
        {
            Names = new List<string> { organization.Trim() },
            Limit = 1
        };

        var command = new OrganizationLookupCommand(lookupRequest.ToKiotaModel());
        var response = await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        if (response?.Orgs is null || response.Orgs.Count == 0)
        {
            return NotFound();
        }

        return Ok(response.Orgs[0]);
    }

    private static bool HasLookupCriteria(OrganizationLookupRequest request)
    {
        return (request.Names is not null && request.Names.Any(static name => !string.IsNullOrWhiteSpace(name))) ||
            (request.Urls is not null && request.Urls.Any(static url => !string.IsNullOrWhiteSpace(url))) ||
            !string.IsNullOrWhiteSpace(request.ProfileUrl);
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
