using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using TearLogic.Clients;
using TearLogic.Clients.Models.Common;
using TearLogic.Clients.Models.V2BusinessRelationships;
using TearLogic.Clients.Models.V2ChatCbi;
using TearLogic.Clients.Models.V2FinancialTransactions;
using TearLogic.Clients.Models.V2Firmographics;
using TearLogic.Clients.Models.V2ManagementAndBoard;
using TearLogic.Clients.Models.V2OrganizationLookup;
using TearLogic.Clients.Models.V2Outlook;
using TearLogic.Clients.Models.V2ScoutingReports;

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
        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogOrganizationLookupStarted();

        try
        {
            var response = await client.V2.Organizations.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogOrganizationLookupCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogOrganizationLookupFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogOrganizationLookupFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<FirmographicsResponse?> GetFirmographicsAsync(FirmographicsRequestBody request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var (client, adapter) = await CreateClientContextAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogFirmographicsLookupStarted();

        try
        {
            var response = await client.V2.Firmographics.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogFirmographicsLookupCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogFirmographicsLookupFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogFirmographicsLookupFailedGeneric(exception);
            throw;
        }
    }
    /// <inheritdoc />
    public async Task<FundingsResponse?> GetFundingsAsync(int organizationId, ListTransactionsForOrganizationRequest request, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        ArgumentNullException.ThrowIfNull(request);
        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogFundingsRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Financialtransactions.Fundings.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogFundingsRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogFundingsRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogFundingsRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<InvestmentsResponse?> GetInvestmentsAsync(int organizationId, ListTransactionsForOrganizationRequest request, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        ArgumentNullException.ThrowIfNull(request);
        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInvestmentsRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Financialtransactions.Investments.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogInvestmentsRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogInvestmentsRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogInvestmentsRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PortfolioExitsResponse?> GetPortfolioExitsAsync(int organizationId, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        var (client, adapter) = await CreateClientContextAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogPortfolioExitsRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Financialtransactions.Portfolioexits.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogPortfolioExitsRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogPortfolioExitsRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogPortfolioExitsRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<BusinessRelationshipsResponse?> GetBusinessRelationshipsAsync(int organizationId, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogBusinessRelationshipsRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Businessrelationships.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogBusinessRelationshipsRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogBusinessRelationshipsRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogBusinessRelationshipsRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ManagementAndBoardResponse?> GetManagementAndBoardAsync(int organizationId, ManagementAndBoardRequestBody request, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        ArgumentNullException.ThrowIfNull(request);
        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogManagementAndBoardRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Managementandboard.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogManagementAndBoardRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogManagementAndBoardRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogManagementAndBoardRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<OutlookResponse?> GetOutlookAsync(int organizationId, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogOutlookRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Outlook.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogOutlookRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogOutlookRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogOutlookRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ScoutingReportResponse?> GetScoutingReportAsync(int organizationId, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogScoutingReportRequestStarted();

        try
        {
            var response = await client.V2.Organizations[organizationId].Scoutingreport.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogScoutingReportRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogScoutingReportRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogScoutingReportRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Stream?> StreamScoutingReportAsync(int organizationId, CancellationToken cancellationToken)
    {
        if (organizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(organizationId), organizationId, "The organization identifier must be a positive integer.");
        }

        var (client, adapter) = await CreateClientContextAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogScoutingReportStreamRequestStarted();

        var requestInfo = client.V2.Organizations[organizationId].Scoutingreportstream.ToPostRequestInformation();
        Dictionary<string, ParsableFactory<IParsable>> errorMapping = new()
        {
            ["400"] = ErrorWithCode.CreateFromDiscriminatorValue,
            ["403"] = ErrorWithCode.CreateFromDiscriminatorValue,
            ["424"] = ErrorWithCode.CreateFromDiscriminatorValue,
            ["500"] = ErrorWithCode.CreateFromDiscriminatorValue,
        };

        try
        {
            var responseStream = await adapter.SendPrimitiveAsync<Stream>(requestInfo, errorMapping, cancellationToken).ConfigureAwait(false);
            _logger.LogScoutingReportStreamRequestCompleted();
            return responseStream;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogScoutingReportStreamRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogScoutingReportStreamRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ChatCbiResponse?> SendChatCbiRequestAsync(ChatCbiRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var client = await CreateClientAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogChatCbiRequestStarted();

        try
        {
            var response = await client.V2.Chatcbi.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogChatCbiRequestCompleted();
            return response;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogChatCbiRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogChatCbiRequestFailedGeneric(exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Stream?> StreamChatCbiAsync(ChatCbiRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var (client, adapter) = await CreateClientContextAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogChatCbiStreamRequestStarted();

        var requestInfo = client.V2.Chatcbichunked.ToPostRequestInformation(request);
        Dictionary<string, ParsableFactory<IParsable>> errorMapping = new()
        {
            ["400"] = ErrorWithCode.CreateFromDiscriminatorValue,
            ["403"] = ErrorWithCode.CreateFromDiscriminatorValue,
            ["424"] = ErrorWithCode.CreateFromDiscriminatorValue,
            ["500"] = ErrorWithCode.CreateFromDiscriminatorValue,
        };

        try
        {
            var responseStream = await adapter.SendPrimitiveAsync<Stream>(requestInfo, errorMapping, cancellationToken).ConfigureAwait(false);
            _logger.LogChatCbiStreamRequestCompleted();
            return responseStream;
        }
        catch (ErrorWithCode exception)
        {
            _logger.LogChatCbiStreamRequestFailed(exception, exception.Error ?? "Unknown");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogChatCbiStreamRequestFailedGeneric(exception);
            throw;
        }
    }

    private async Task<(CBInsightsApiClient Client, IRequestAdapter RequestAdapter)> CreateClientContextAsync(CancellationToken cancellationToken)
    {
        var options = _optionsMonitor.CurrentValue;
        var adapter = await _requestAdapterFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
        adapter.BaseUrl = options.BaseUrl;
        var client = new CBInsightsApiClient(adapter);
        return (client, adapter);
    }

    private async Task<CBInsightsApiClient> CreateClientAsync(CancellationToken cancellationToken)
    {
        var (client, _) = await CreateClientContextAsync(cancellationToken).ConfigureAwait(false);
        return client;
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
