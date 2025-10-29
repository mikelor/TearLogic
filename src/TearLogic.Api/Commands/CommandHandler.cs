namespace TearLogic.Api.CBInsights.Commands;

/// <summary>
/// Provides a base abstraction for handling commands.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResult">The command result type.</typeparam>
public abstract class CommandHandler<TCommand, TResult>(ILogger logger) : ICommandHandler<TCommand, TResult>
{
    /// <summary>
    /// Gets the logger instance for derived handlers.
    /// </summary>
    protected ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public abstract Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Defines an abstraction for handling commands.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
public interface ICommandHandler<in TCommand, TResult>
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="command">The command payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command result.</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
