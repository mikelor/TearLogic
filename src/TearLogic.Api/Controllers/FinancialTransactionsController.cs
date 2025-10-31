using System.Linq;
using TearLogic.Api.CBInsights.Requests;
using TearLogic.Clients.Models.V2BusinessRelationships;
using TearLogic.Clients.Models.V2FinancialTransactions;
using TearLogic.Clients.Models.V2ManagementAndBoard;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for CB Insights financial transaction operations.
/// </summary>
[ApiController]
[Route("api/cbinsights/organizations")]
public sealed class FinancialTransactionsController(
    IFundingsCommandHandler fundingsCommandHandler,
    IInvestmentsCommandHandler investmentsCommandHandler,
    IPortfolioExitsCommandHandler portfolioExitsCommandHandler,
    IBusinessRelationshipsCommandHandler businessRelationshipsCommandHandler,
    IManagementAndBoardCommandHandler managementAndBoardCommandHandler
) : ControllerBase
{
    private readonly IFundingsCommandHandler _fundingsCommandHandler = fundingsCommandHandler ?? throw new ArgumentNullException(nameof(fundingsCommandHandler));
    private readonly IInvestmentsCommandHandler _investmentsCommandHandler = investmentsCommandHandler ?? throw new ArgumentNullException(nameof(investmentsCommandHandler));
    private readonly IPortfolioExitsCommandHandler _portfolioExitsCommandHandler = portfolioExitsCommandHandler ?? throw new ArgumentNullException(nameof(portfolioExitsCommandHandler));
    private readonly IBusinessRelationshipsCommandHandler _businessRelationshipsCommandHandler = businessRelationshipsCommandHandler ?? throw new ArgumentNullException(nameof(businessRelationshipsCommandHandler));
    private readonly IManagementAndBoardCommandHandler _managementAndBoardCommandHandler = managementAndBoardCommandHandler ?? throw new ArgumentNullException(nameof(managementAndBoardCommandHandler));

    /// <summary>
    /// Retrieves funding transactions for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="request">The optional pagination request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The funding transactions if they exist.</returns>
    [HttpGet("{organizationId:int}/financialtransactions/fundings")]
    [ProducesResponseType(typeof(FundingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFundingsAsync(int? organizationId, [FromQuery] FinancialTransactionsListRequest? request, CancellationToken cancellationToken)
    {
        if (!ValidateOrganizationId(organizationId))
        {
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var paginationRequest = request ?? new FinancialTransactionsListRequest();
        var command = new FundingsCommand(organizationId.Value, paginationRequest.ToKiotaModel());
        var response = await _fundingsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves investment transactions for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="request">The optional pagination request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The investment transactions if they exist.</returns>
    [HttpGet("{organizationId:int}/financialtransactions/investments")]
    [ProducesResponseType(typeof(InvestmentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInvestmentsAsync(int? organizationId, [FromQuery] FinancialTransactionsListRequest? request, CancellationToken cancellationToken)
    {
        if (!ValidateOrganizationId(organizationId))
        {
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var paginationRequest = request ?? new FinancialTransactionsListRequest();
        var command = new InvestmentsCommand(organizationId.Value, paginationRequest.ToKiotaModel());
        var response = await _investmentsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves portfolio exit transactions for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The portfolio exit transactions if they exist.</returns>
    [HttpGet("{organizationId:int}/financialtransactions/portfolioexits")]
    [ProducesResponseType(typeof(PortfolioExitsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPortfolioExitsAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!ValidateOrganizationId(organizationId))
        {
            return ValidationProblem(ModelState);
        }

        var command = new PortfolioExitsCommand(organizationId.Value);
        var response = await _portfolioExitsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves business relationships for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The business relationships if they exist.</returns>
    [HttpGet("{organizationId:int}/financialtransactions/businessrelationships")]
    [ProducesResponseType(typeof(BusinessRelationshipsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBusinessRelationshipsAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!ValidateOrganizationId(organizationId))
        {
            return ValidationProblem(ModelState);
        }

        var command = new BusinessRelationshipsCommand(organizationId.Value);
        var response = await _businessRelationshipsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Retrieves management and board details for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="request">The optional filter request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The management and board details if they exist.</returns>
    [HttpGet("{organizationId:int}/financialtransactions/managementandboard")]
    [ProducesResponseType(typeof(ManagementAndBoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetManagementAndBoardAsync(int? organizationId, [FromQuery] ManagementAndBoardRequest? request, CancellationToken cancellationToken)
    {
        if (!ValidateOrganizationId(organizationId))
        {
            return ValidationProblem(ModelState);
        }

        if (request?.TitleIds?.Any(id => id <= 0) == true)
        {
            ModelState.AddModelError(nameof(ManagementAndBoardRequest.TitleIds), "Title identifiers must be positive integers.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var managementRequest = request ?? new ManagementAndBoardRequest();
        var command = new ManagementAndBoardCommand(organizationId.Value, managementRequest.ToKiotaModel());
        var response = await _managementAndBoardCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    private bool ValidateOrganizationId(int? organizationId)
    {
        if (!organizationId.HasValue || organizationId.Value <= 0)
        {
            ModelState.AddModelError(nameof(organizationId), "The organization identifier must be a positive integer.");
            return false;
        }

        return true;
    }
}

/// <summary>
/// Defines the command handler abstraction for retrieving funding transactions.
/// </summary>
public interface IFundingsCommandHandler : ICommandHandler<FundingsCommand, FundingsResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving investment transactions.
/// </summary>
public interface IInvestmentsCommandHandler : ICommandHandler<InvestmentsCommand, InvestmentsResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving portfolio exit transactions.
/// </summary>
public interface IPortfolioExitsCommandHandler : ICommandHandler<PortfolioExitsCommand, PortfolioExitsResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving business relationships.
/// </summary>
public interface IBusinessRelationshipsCommandHandler : ICommandHandler<BusinessRelationshipsCommand, BusinessRelationshipsResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for retrieving management and board details.
/// </summary>
public interface IManagementAndBoardCommandHandler : ICommandHandler<ManagementAndBoardCommand, ManagementAndBoardResponse?>
{
}

/// <summary>
/// Handles funding transaction retrieval commands.
/// </summary>
public sealed class FundingsCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<FundingsCommandHandler> logger
) : CommandHandler<FundingsCommand, FundingsResponse?>(logger), IFundingsCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<FundingsResponse?> HandleAsync(FundingsCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.GetFundingsAsync(command.OrganizationId, command.Request, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles investment transaction retrieval commands.
/// </summary>
public sealed class InvestmentsCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<InvestmentsCommandHandler> logger
) : CommandHandler<InvestmentsCommand, InvestmentsResponse?>(logger), IInvestmentsCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<InvestmentsResponse?> HandleAsync(InvestmentsCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.GetInvestmentsAsync(command.OrganizationId, command.Request, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles portfolio exit retrieval commands.
/// </summary>
public sealed class PortfolioExitsCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<PortfolioExitsCommandHandler> logger
) : CommandHandler<PortfolioExitsCommand, PortfolioExitsResponse?>(logger), IPortfolioExitsCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<PortfolioExitsResponse?> HandleAsync(PortfolioExitsCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.GetPortfolioExitsAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles business relationship retrieval commands.
/// </summary>
public sealed class BusinessRelationshipsCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<BusinessRelationshipsCommandHandler> logger
) : CommandHandler<BusinessRelationshipsCommand, BusinessRelationshipsResponse?>(logger), IBusinessRelationshipsCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<BusinessRelationshipsResponse?> HandleAsync(BusinessRelationshipsCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.GetBusinessRelationshipsAsync(command.OrganizationId, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles management and board retrieval commands.
/// </summary>
public sealed class ManagementAndBoardCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<ManagementAndBoardCommandHandler> logger
) : CommandHandler<ManagementAndBoardCommand, ManagementAndBoardResponse?>(logger), IManagementAndBoardCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<ManagementAndBoardResponse?> HandleAsync(ManagementAndBoardCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Request);
        return await _cbInsightsClient.GetManagementAndBoardAsync(command.OrganizationId, command.Request, cancellationToken).ConfigureAwait(false);
    }
}
