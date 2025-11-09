using System.IO;
using TearLogic.Api.CBInsights.Requests;
using ClientChatCbiResponse = TearLogic.Clients.Models.V2ChatCbi.ChatCbiResponse;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Exposes endpoints for CB Insights ChatCbi operations.
/// </summary>
[ApiController]
[Route("api/cbinsights/chat")]
public sealed class ChatCbiController(
    IChatCbiCommandHandler chatCommandHandler,
    IChatCbiStreamCommandHandler chatStreamCommandHandler
) : ControllerBase
{
    private readonly IChatCbiCommandHandler _chatCommandHandler = chatCommandHandler ?? throw new ArgumentNullException(nameof(chatCommandHandler));
    private readonly IChatCbiStreamCommandHandler _chatStreamCommandHandler = chatStreamCommandHandler ?? throw new ArgumentNullException(nameof(chatStreamCommandHandler));

    /// <summary>
    /// Sends a message to ChatCbi and returns the generated response.
    /// </summary>
    /// <param name="request">The ChatCbi request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ChatCbi response.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClientChatCbiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] ChatCbiRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            ModelState.AddModelError(nameof(request.Message), "The message must be provided.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new ChatCbiCommand(request.ToKiotaModel());
        var response = await _chatCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Sends a message to ChatCbi and streams the response as JSON chunks.
    /// </summary>
    /// <param name="request">The ChatCbi request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The streaming response.</returns>
    [HttpPost("stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StreamAsync([FromBody] ChatCbiRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            ModelState.AddModelError(nameof(request.Message), "The message must be provided.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new ChatCbiStreamCommand(request.ToKiotaModel());
        var responseStream = await _chatStreamCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        if (responseStream is null)
        {
            return NotFound();
        }

        return File(responseStream, "application/json");
    }
}

/// <summary>
/// Defines the command handler abstraction for ChatCbi requests.
/// </summary>
public interface IChatCbiCommandHandler : ICommandHandler<ChatCbiCommand, ClientChatCbiResponse?>
{
}

/// <summary>
/// Defines the command handler abstraction for streaming ChatCbi requests.
/// </summary>
public interface IChatCbiStreamCommandHandler : ICommandHandler<ChatCbiStreamCommand, Stream?>
{
}

/// <summary>
/// Handles ChatCbi commands.
/// </summary>
public sealed class ChatCbiCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<ChatCbiCommandHandler> logger
) : CommandHandler<ChatCbiCommand, ClientChatCbiResponse?>(logger), IChatCbiCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override async Task<ClientChatCbiResponse?> HandleAsync(ChatCbiCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await _cbInsightsClient.SendChatCbiRequestAsync(command.Request, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Handles streaming ChatCbi commands.
/// </summary>
public sealed class ChatCbiStreamCommandHandler(
    ICBInsightsClient cbInsightsClient,
    ILogger<ChatCbiStreamCommandHandler> logger
) : CommandHandler<ChatCbiStreamCommand, Stream?>(logger), IChatCbiStreamCommandHandler
{
    private readonly ICBInsightsClient _cbInsightsClient = cbInsightsClient ?? throw new ArgumentNullException(nameof(cbInsightsClient));

    /// <inheritdoc />
    public override Task<Stream?> HandleAsync(ChatCbiStreamCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return _cbInsightsClient.StreamChatCbiAsync(command.Request, cancellationToken);
    }
}
