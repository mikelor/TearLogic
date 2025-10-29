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
    /// Retrieves basic profile information for organizations that match the supplied names.
    /// </summary>
    /// <param name="names">The organization names to look up. Provide the parameter multiple times to query more than one organization.</param>
    /// <param name="limit">The maximum number of organizations to return. Must be between 1 and 100. Defaults to the number of provided names, up to 100.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The lookup response that contains the matching organizations.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(OrgLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync([FromQuery(Name = "names")] IEnumerable<string>? names, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var normalizedNames = names?
            .Where(static name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedNames is null || normalizedNames.Count == 0)
        {
            ModelState.AddModelError(nameof(names), "At least one organization name must be provided.");
        }

        if (limit.HasValue && (limit.Value < 1 || limit.Value > 100))
        {
            ModelState.AddModelError(nameof(limit), "The limit must be between 1 and 100.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var requestBody = new OrgLookupRequestBody
        {
            Names = normalizedNames,
            Limit = limit ?? Math.Min(normalizedNames.Count, 100)
        };

        var command = new OrganizationLookupCommand(requestBody);
        var response = await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        response ??= new OrgLookupResponse
        {
            Orgs = new List<Org>(),
            TotalHits = 0,
            TotalHitsRelation = "eq"
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a single organization profile that matches the supplied organization name.
    /// </summary>
    /// <param name="organization">The organization name to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The organization profile if a match is found.</returns>
    [HttpGet("{organization}")]
    [ProducesResponseType(typeof(Org), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByOrganizationAsync(string? organization, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organization))
        {
            ModelState.AddModelError(nameof(organization), "The organization name must be provided.");
            return ValidationProblem(ModelState);
        }

        var requestBody = new OrgLookupRequestBody
        {
            Names = new List<string> { organization },
            Limit = 1
        };

        var command = new OrganizationLookupCommand(requestBody);
        var response = await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        var organizationMatch = response?.Orgs?.FirstOrDefault(static org => !string.IsNullOrWhiteSpace(org?.Name));

        if (organizationMatch is null)
        {
            return NotFound();
        }

        if (!string.Equals(organizationMatch.Name, organization, StringComparison.OrdinalIgnoreCase))
        {
            var exactMatch = response?.Orgs?
                .FirstOrDefault(org => !string.IsNullOrWhiteSpace(org?.Name) && string.Equals(org.Name, organization, StringComparison.OrdinalIgnoreCase));

            organizationMatch = exactMatch ?? organizationMatch;
        }

        return Ok(organizationMatch);
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
