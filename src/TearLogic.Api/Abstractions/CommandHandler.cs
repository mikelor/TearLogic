namespace Service.CBInsights.Abstractions;

/// <summary>
/// Provides a base abstraction for asynchronous command handling operations.
/// </summary>
/// <typeparam name="TCommand">The command payload type.</typeparam>
/// <typeparam name="TResult">The response payload type.</typeparam>
public abstract class CommandHandler<TCommand, TResult>
{
    /// <summary>
    /// Executes the command using the supplied cancellation token.
    /// </summary>
    /// <param name="command">The command payload to process.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the command execution.</param>
    /// <returns>A task that resolves to the command result.</returns>
    public abstract Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
