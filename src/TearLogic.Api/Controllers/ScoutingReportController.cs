using System.IO;
using TearLogic.Api.CBInsights.Commands.ScoutingReports;
using TearLogic.Clients.Models.V2ScoutingReports;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for CB Insights scouting report operations.
/// </summary>
[ApiController]
[Route("api/cbinsights/organizations")]
public sealed class ScoutingReportController(
    IScoutingReportCommandHandler scoutingReportCommandHandler,
    IScoutingReportStreamCommandHandler scoutingReportStreamCommandHandler
) : ControllerBase
{
    private readonly IScoutingReportCommandHandler _scoutingReportCommandHandler = scoutingReportCommandHandler ?? throw new ArgumentNullException(nameof(scoutingReportCommandHandler));
    private readonly IScoutingReportStreamCommandHandler _scoutingReportStreamCommandHandler = scoutingReportStreamCommandHandler ?? throw new ArgumentNullException(nameof(scoutingReportStreamCommandHandler));

    /// <summary>
    /// Retrieves the scouting report for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The scouting report when available.</returns>
    [HttpGet("{organizationId:int}/scoutingreport")]
    [ProducesResponseType(typeof(ScoutingReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetScoutingReportAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new ScoutingReportCommand(organizationIdValue);
        var response = await _scoutingReportCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Streams the scouting report for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The streaming response.</returns>
    [HttpGet("{organizationId:int}/scoutingreport/stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StreamScoutingReportAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new ScoutingReportStreamCommand(organizationIdValue);
        var responseStream = await _scoutingReportStreamCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (responseStream is null)
        {
            return NotFound();
        }

        return File(responseStream, "application/json");
    }
}

/// <summary>
/// Defines the command handler abstraction for retrieving scouting reports.
/// </summary>
public interface IScoutingReportCommandHandler : ICommandHandler<ScoutingReportCommand, ScoutingReportResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for streaming scouting reports.
/// </summary>
public interface IScoutingReportStreamCommandHandler : ICommandHandler<ScoutingReportStreamCommand, Stream?>
{
}

/// <summary>
/// Handles scouting report retrieval commands.
/// </summary>
public sealed class ScoutingReportCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<ScoutingReportCommandHandler> logger
) : CommandHandler<ScoutingReportCommand, ScoutingReportResponse?>(logger), IScoutingReportCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<ScoutingReportResponse?> HandleAsync(ScoutingReportCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.GetScoutingReportAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles scouting report streaming commands.
/// </summary>
public sealed class ScoutingReportStreamCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<ScoutingReportStreamCommandHandler> logger
) : CommandHandler<ScoutingReportStreamCommand, Stream?>(logger), IScoutingReportStreamCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override Task<Stream?> HandleAsync(ScoutingReportStreamCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return _cbInsightsClient.StreamScoutingReportAsync(command.OrganizationId, cancellationToken);
    }
}
