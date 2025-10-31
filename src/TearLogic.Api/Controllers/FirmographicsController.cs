using System.Collections.Generic;
using System.Linq;
using TearLogic.Clients.Models.V2Firmographics;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for CB Insights firmographics operations.
/// </summary>
[ApiController]
[Route("api/cbinsights/organizations")]
public sealed class FirmographicsController
(
    IFirmographicsCommandHandler commandHandler
) : ControllerBase
{
    private readonly IFirmographicsCommandHandler _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));

    /// <summary>
    /// Retrieves a firmographics profile for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The firmographics profile for the organization if it exists.</returns>
    [HttpGet("{organizationId:int}/firmographics")]
    [ProducesResponseType(typeof(Org), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var requestBody = new FirmographicsRequestBody
        {
            OrgIds = new List<int?> { organizationIdValue },
            Limit = 1
        };

        var command = new FirmographicsCommand(requestBody);
        var response = await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        var firmographics = response?.Orgs?.FirstOrDefault();

        if (firmographics is null)
        {
            return NotFound();
        }

        return Ok(firmographics);
    }
}

/// <summary>
/// Defines the command handler abstraction for firmographics lookups.
/// </summary>
public interface IFirmographicsCommandHandler : ICommandHandler<FirmographicsCommand, FirmographicsResponse?>
{
}

/// <summary>
/// Handles the firmographics lookup command.
/// </summary>
public sealed class FirmographicsCommandHandler
(
    ICBInsightsClient cbInsightsClient,
    ILogger<FirmographicsCommandHandler> logger
) : CommandHandler<FirmographicsCommand, FirmographicsResponse?>(logger), IFirmographicsCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<FirmographicsResponse?> HandleAsync(FirmographicsCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var response = await _cbInsightsClient.GetFirmographicsAsync(command.Request, cancellationToken).ConfigureAwait(false);
        return response;
    }
}
