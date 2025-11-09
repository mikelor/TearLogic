namespace TearLogic.Api.CBInsights.Commands.ChatCbi;

/// <summary>
/// Represents a command that requests a streaming ChatCbi response.
/// </summary>
/// <param name="Request">The ChatCbi request payload.</param>
public sealed record ChatCbiStreamCommand(global::TearLogic.Clients.Models.V2ChatCbi.ChatCbiRequest Request);
