using System.Resources;

namespace TearLogic.Api.CBInsights.Diagnostics;

/// <summary>
/// Provides access to localized log messages.
/// </summary>
public interface ILogMessageProvider
{
    /// <summary>
    /// Retrieves a localized message for the provided key.
    /// </summary>
    /// <param name="name">The resource name.</param>
    /// <returns>The localized message.</returns>
    string? GetString(string name);
}

/// <summary>
/// Provides access to localized error messages.
/// </summary>
public interface IErrorMessageProvider
{
    /// <summary>
    /// Retrieves a localized message for the provided key.
    /// </summary>
    /// <param name="name">The resource name.</param>
    /// <returns>The localized message.</returns>
    string? GetString(string name);
}

/// <summary>
/// Default implementation for <see cref="ILogMessageProvider"/>.
/// </summary>
public sealed class LogMessageProvider : ILogMessageProvider
{
    private static readonly ResourceManager ResourceManager = new("TearLogic.Api.CBInsights.Resources.LogMessages", typeof(LogMessageProvider).Assembly);

    /// <inheritdoc />
    public string? GetString(string name) => ResourceManager.GetString(name);
}

/// <summary>
/// Default implementation for <see cref="IErrorMessageProvider"/>.
/// </summary>
public sealed class ErrorMessageProvider : IErrorMessageProvider
{
    private static readonly ResourceManager ResourceManager = new("TearLogic.Api.CBInsights.Resources.ErrorMessages", typeof(ErrorMessageProvider).Assembly);

    /// <inheritdoc />
    public string? GetString(string name) => ResourceManager.GetString(name);
}
