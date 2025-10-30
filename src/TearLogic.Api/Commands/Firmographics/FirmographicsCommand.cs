using TearLogic.Clients.Models.V2Firmographics;

namespace TearLogic.Api.CBInsights.Commands.Firmographics;

/// <summary>
/// Represents a firmographics lookup command.
/// </summary>
/// <param name="Request">The CB Insights request body.</param>
public sealed record FirmographicsCommand(FirmographicsRequestBody Request);
