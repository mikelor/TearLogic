namespace TearLogic.Api.CBInsights.Commands.ChatCbi;

/// <summary>
/// Represents a command that invokes the ChatCbi API.
/// </summary>
/// <param name="Request">The ChatCbi request payload.</param>
public sealed record ChatCbiCommand(global::TearLogic.Clients.Models.V2ChatCbi.ChatCbiRequest Request);
