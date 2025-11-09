using System.Linq;
using TearLogic.Api.CBInsights.Requests;
using TearLogic.Clients.Models.V2BusinessRelationships;
using TearLogic.Clients.Models.V2ManagementAndBoard;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for retrieving CB Insights organization relationships.
/// </summary>
[ApiController]
[Route("api/cbinsights/organizations")]
public sealed class OrganizationRelationshipsController(
    IBusinessRelationshipsCommandHandler businessRelationshipsCommandHandler,
    IManagementAndBoardCommandHandler managementAndBoardCommandHandler
) : ControllerBase
{
    private readonly IBusinessRelationshipsCommandHandler _businessRelationshipsCommandHandler = businessRelationshipsCommandHandler ?? throw new ArgumentNullException(nameof(businessRelationshipsCommandHandler));
    private readonly IManagementAndBoardCommandHandler _managementAndBoardCommandHandler = managementAndBoardCommandHandler ?? throw new ArgumentNullException(nameof(managementAndBoardCommandHandler));

    /// <summary>
    /// Retrieves business relationships for the supplied CB Insights organization identifier.
    /// </summary>
    /// <param name="organizationId">The CB Insights organization identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The business relationships if they exist.</returns>
    [HttpGet("{organizationId:int}/businessrelationships")]
    [ProducesResponseType(typeof(BusinessRelationshipsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBusinessRelationshipsAsync(int? organizationId, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
        {
            return ValidationProblem(ModelState);
        }

        var command = new BusinessRelationshipsCommand(organizationIdValue);
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
    [HttpGet("{organizationId:int}/managementandboard")]
    [ProducesResponseType(typeof(ManagementAndBoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetManagementAndBoardAsync(int? organizationId, [FromQuery] ManagementAndBoardRequest? request, CancellationToken cancellationToken)
    {
        if (!this.TryValidateOrganizationId(organizationId, out var organizationIdValue))
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
        var command = new ManagementAndBoardCommand(organizationIdValue, managementRequest.ToKiotaModel());
        var response = await _managementAndBoardCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}
