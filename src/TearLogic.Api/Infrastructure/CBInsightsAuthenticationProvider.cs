using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace TearLogic.Api.CBInsights.Infrastructure;

/// <summary>
/// Provides authentication for CB Insights requests.
/// </summary>
public interface ICBInsightsAuthenticationProvider : IAuthenticationProvider
{
}

/// <summary>
/// Adds the CB Insights bearer token to outgoing requests.
/// </summary>
public sealed class CBInsightsAuthenticationProvider
(
    ICBInsightsTokenProvider tokenProvider
) : ICBInsightsAuthenticationProvider
{
    private readonly ICBInsightsTokenProvider _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));

    /// <inheritdoc />
    public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var token = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        request.Headers["Authorization"] = new[] { $"Bearer {token}" };
    }
}
