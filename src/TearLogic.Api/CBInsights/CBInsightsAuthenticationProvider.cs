using Microsoft.Kiota.Abstractions;

namespace TearLogic.Api.CBInsights;

internal sealed class CBInsightsAuthenticationProvider : IAuthenticationProvider
{
    private readonly ICBInsightsTokenService _tokenService;

    public CBInsightsAuthenticationProvider(ICBInsightsTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var token = await _tokenService.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.TryAdd("Authorization", $"Bearer {token}");
        }
    }
}
