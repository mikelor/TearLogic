using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using TearLogic.Api.Services.Abstractions;

namespace TearLogic.Api.Services.Internal;

internal sealed class CBInsightsAuthenticationProvider : IAuthenticationProvider
{
    private readonly ICBInsightsTokenProvider _tokenProvider;

    public CBInsightsAuthenticationProvider(ICBInsightsTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var token = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        var headerValue = $"Bearer {token}";

        if (!request.Headers.TryAdd("Authorization", headerValue))
        {
            request.Headers["Authorization"] = headerValue;
        }
    }
}
