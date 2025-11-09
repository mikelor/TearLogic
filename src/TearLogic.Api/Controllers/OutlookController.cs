using TearLogic.Api.CBInsights.Commands.Outlook;
using TearLogic.Clients.Models.V2Outlook;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for CB Insights outlook operations.
/// </summary>
[ApiController]
[Route("api/cbinsights/organizations")]
public sealed class OutlookController(
    IOutlookCommandHandler outlookCommandHandler,
    ICommercialMaturityCommandHandler commercialMaturityCommandHandler,
    IExitProbabilityCommandHandler exitProbabilityCommandHandler,
    IMosaicScoreCommandHandler mosaicScoreCommandHandler
) : ControllerBase
{
    private readonly IOutlookCommandHandler _outlookCommandHandler = outlookCommandHandler ?? throw new ArgumentNullException(nameof(outlookCommandHandler));
    private readonly ICommercialMaturityCommandHandler _commercialMaturityCommandHandler = commercialMaturityCommandHandler ?? throw new ArgumentNullException(nameof(commercialMaturityCommandHandler));
    private readonly IExitProbabilityCommandHandler _exitProbabilityCommandHandler = exitProbabilityCommandHandler ?? throw new ArgumentNullException(nameof(exitProbabilityCommandHandler));
    private readonly IMosaicScoreCommandHandler _mosaicScoreCommandHandler = mosaicScoreCommandHandler ?? throw new ArgumentNullException(nameof(mosaicScoreCommandHandler));

    /// <summary>
    /// Retrieves outlook information for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outlook details if they exist.</returns>
    [HttpGet("{organizationId:int}/outlook")]
    [ProducesResponseType(typeof(OutlookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOutlookAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new OutlookCommand(organizationIdValue);
        var response = await _outlookCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves the commercial maturity for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The commercial maturity if it exists.</returns>
    [HttpGet("{organizationId:int}/outlook/commercialmaturity")]
    [ProducesResponseType(typeof(CommercialMaturity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCommercialMaturityAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new CommercialMaturityCommand(organizationIdValue);
        var response = await _commercialMaturityCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves the exit probability for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit probability if it exists.</returns>
    [HttpGet("{organizationId:int}/outlook/exitprobability")]
    [ProducesResponseType(typeof(CurrentExitProbability), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExitProbabilityAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new ExitProbabilityCommand(organizationIdValue);
        var response = await _exitProbabilityCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves the Mosaic score for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Mosaic score if it exists.</returns>
    [HttpGet("{organizationId:int}/outlook/mosaic")]
    [ProducesResponseType(typeof(MosaicScore), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMosaicScoreAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new MosaicScoreCommand(organizationIdValue);
        var response = await _mosaicScoreCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}

/// <summary>
/// Defines the command handler abstraction for retrieving outlook information.
/// </summary>
public interface IOutlookCommandHandler : ICommandHandler<OutlookCommand, OutlookResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving commercial maturity information.
/// </summary>
public interface ICommercialMaturityCommandHandler : ICommandHandler<CommercialMaturityCommand, CommercialMaturity?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving exit probability information.
/// </summary>
public interface IExitProbabilityCommandHandler : ICommandHandler<ExitProbabilityCommand, CurrentExitProbability?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving Mosaic score information.
/// </summary>
public interface IMosaicScoreCommandHandler : ICommandHandler<MosaicScoreCommand, MosaicScore?>
{
}

/// <summary>
/// Handles outlook retrieval commands.
/// </summary>
public sealed class OutlookCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<OutlookCommandHandler> logger
) : CommandHandler<OutlookCommand, OutlookResponse?>(logger), IOutlookCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<OutlookResponse?> HandleAsync(OutlookCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.GetOutlookAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles commercial maturity retrieval commands.
/// </summary>
public sealed class CommercialMaturityCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<CommercialMaturityCommandHandler> logger
) : CommandHandler<CommercialMaturityCommand, CommercialMaturity?>(logger), ICommercialMaturityCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<CommercialMaturity?> HandleAsync(CommercialMaturityCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var outlook = await _cbInsightsClient.GetOutlookAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
        return outlook?.CommercialMaturity;
    }
}

/// <summary>
/// Handles exit probability retrieval commands.
/// </summary>
public sealed class ExitProbabilityCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<ExitProbabilityCommandHandler> logger
) : CommandHandler<ExitProbabilityCommand, CurrentExitProbability?>(logger), IExitProbabilityCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<CurrentExitProbability?> HandleAsync(ExitProbabilityCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var outlook = await _cbInsightsClient.GetOutlookAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
        return outlook?.ExitProbability;
    }
}

/// <summary>
/// Handles Mosaic score retrieval commands.
/// </summary>
public sealed class MosaicScoreCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<MosaicScoreCommandHandler> logger
) : CommandHandler<MosaicScoreCommand, MosaicScore?>(logger), IMosaicScoreCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<MosaicScore?> HandleAsync(MosaicScoreCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var outlook = await _cbInsightsClient.GetOutlookAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
        return outlook?.MosaicScore;
    }
}
