using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Http.HttpClientLibrary;
using TearLogic.Clients;
using TearLogic.Clients.Models.Common;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.CBInsights.Infrastructure;

/// <summary>
/// Provides Kiota-based access to the CB Insights API.
/// </summary>
public sealed class CBInsightsClient
(
    ICBInsightsRequestAdapterFactory requestAdapterFactory,
    IOptionsMonitor<CBInsightsOptions> optionsMonitor,
    ILogger<CBInsightsClient> logger,
    IErrorMessageProvider errorMessageProvider,
    ILogMessageProvider logMessageProvider
) : ICBInsightsClient
{
    private readonly IOptionsMonitor<CBInsightsOptions> _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
    private readonly ILogger<CBInsightsClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IErrorMessageProvider _errorMessageProvider = errorMessageProvider ?? throw new ArgumentNullException(nameof(errorMessageProvider));
    private readonly ILogMessageProvider _logMessageProvider = logMessageProvider ?? throw new ArgumentNullException(nameof(logMessageProvider));
    private readonly ICBInsightsRequestAdapterFactory _requestAdapterFactory = requestAdapterFactory ?? throw new ArgumentNullException(nameof(requestAdapterFactory));

    /// <inheritdoc />
    public async Task<OrgLookupResponse?> LookupOrganizationAsync(OrgLookupRequestBody request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var options = _optionsMonitor.CurrentValue;
        var adapter = await _requestAdapterFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
        adapter.BaseUrl = options.BaseUrl;
        var client = new CBInsightsApiClient(adapter);
        var message = _logMessageProvider.GetString("OrganizationLookupStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("OrganizationLookupCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("OrganizationLookupFailed") ?? "CB Insights organization lookup failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("OrganizationLookupFailed") ?? "CB Insights organization lookup failed.";
            _logger.LogError(exception, errorMessage);
            throw;
        }
    }
}

/// <summary>
/// Defines a factory capable of producing request adapters configured for CB Insights.
/// </summary>
public interface ICBInsightsRequestAdapterFactory
{
    /// <summary>
    /// Creates a request adapter instance.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request adapter.</returns>
    Task<IRequestAdapter> CreateAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Produces configured Kiota request adapters.
/// </summary>
public sealed class CBInsightsRequestAdapterFactory
(
    IHttpClientFactory httpClientFactory,
    ICBInsightsAuthenticationProvider authenticationProvider
) : ICBInsightsRequestAdapterFactory
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly ICBInsightsAuthenticationProvider _authenticationProvider = authenticationProvider ?? throw new ArgumentNullException(nameof(authenticationProvider));

    /// <inheritdoc />
    public Task<IRequestAdapter> CreateAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var httpClient = _httpClientFactory.CreateClient(CBInsightsHttpClientNames.Api);
        IRequestAdapter adapter = new HttpClientRequestAdapter(_authenticationProvider, httpClient: httpClient);
        return Task.FromResult(adapter);
    }
}
