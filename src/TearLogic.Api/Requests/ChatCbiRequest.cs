using System.ComponentModel.DataAnnotations;

namespace TearLogic.Api.CBInsights.Requests;

/// <summary>
/// Represents a request payload for invoking ChatCbi conversations.
/// </summary>
public sealed class ChatCbiRequest
{
    /// <summary>
    /// Gets or sets the conversation identifier used for multi-turn exchanges.
    /// </summary>
    public string? ChatId { get; set; }

    /// <summary>
    /// Gets or sets the message to send to ChatCbi.
    /// </summary>
    [Required]
    public string? Message { get; set; }

    /// <summary>
    /// Converts this request into the Kiota ChatCbi model.
    /// </summary>
    /// <returns>The Kiota request model.</returns>
    public global::TearLogic.Clients.Models.V2ChatCbi.ChatCbiRequest ToKiotaModel()
    {
        return new global::TearLogic.Clients.Models.V2ChatCbi.ChatCbiRequest
        {
            ChatID = ChatId,
            Message = Message
        };
    }
}
