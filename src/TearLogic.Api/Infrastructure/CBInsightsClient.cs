using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using TearLogic.Clients;
using TearLogic.Clients.Models.Common;
using TearLogic.Clients.Models.V2BusinessRelationships;
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

    /// <inheritdoc />
    public async Task<FirmographicsResponse?> GetFirmographicsAsync(FirmographicsRequestBody request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var (client, adapter) = await CreateClientContextAsync(cancellationToken).ConfigureAwait(false);
        var message = _logMessageProvider.GetString("FirmographicsLookupStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Firmographics.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("FirmographicsLookupCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("FirmographicsLookupFailed") ?? "CB Insights firmographics lookup failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("FirmographicsLookupFailed") ?? "CB Insights firmographics lookup failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("FundingsRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Financialtransactions.Fundings.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("FundingsRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("FundingsRequestFailed") ?? "CB Insights fundings request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("FundingsRequestFailed") ?? "CB Insights fundings request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("InvestmentsRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Financialtransactions.Investments.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("InvestmentsRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("InvestmentsRequestFailed") ?? "CB Insights investments request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("InvestmentsRequestFailed") ?? "CB Insights investments request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("PortfolioExitsRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Financialtransactions.Portfolioexits.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("PortfolioExitsRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("PortfolioExitsRequestFailed") ?? "CB Insights portfolio exits request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("PortfolioExitsRequestFailed") ?? "CB Insights portfolio exits request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("BusinessRelationshipsRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Businessrelationships.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("BusinessRelationshipsRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("BusinessRelationshipsRequestFailed") ?? "CB Insights business relationships request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("BusinessRelationshipsRequestFailed") ?? "CB Insights business relationships request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("ManagementAndBoardRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Managementandboard.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("ManagementAndBoardRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("ManagementAndBoardRequestFailed") ?? "CB Insights management and board request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("ManagementAndBoardRequestFailed") ?? "CB Insights management and board request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("OutlookRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Outlook.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("OutlookRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("OutlookRequestFailed") ?? "CB Insights outlook request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("OutlookRequestFailed") ?? "CB Insights outlook request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("ScoutingReportRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        try
        {
            var response = await client.V2.Organizations[organizationId].Scoutingreport.PostAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("ScoutingReportRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return response;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("ScoutingReportRequestFailed") ?? "CB Insights scouting report request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("ScoutingReportRequestFailed") ?? "CB Insights scouting report request failed.";
            _logger.LogError(exception, errorMessage);
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
        var message = _logMessageProvider.GetString("ScoutingReportStreamRequestStarted");
        if (!string.IsNullOrWhiteSpace(message))
        {
            _logger.LogInformation(message);
        }

        var requestInfo = client.V2.Organizations[organizationId].Scoutingreportstream.ToPostRequestInformation();
        var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
        {
            { "400", ErrorWithCode.CreateFromDiscriminatorValue },
            { "403", ErrorWithCode.CreateFromDiscriminatorValue },
            { "424", ErrorWithCode.CreateFromDiscriminatorValue },
            { "500", ErrorWithCode.CreateFromDiscriminatorValue },
        };

        try
        {
            var responseStream = await adapter.SendPrimitiveAsync<Stream>(requestInfo, errorMapping, cancellationToken).ConfigureAwait(false);
            message = _logMessageProvider.GetString("ScoutingReportStreamRequestCompleted");
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogInformation(message);
            }

            return responseStream;
        }
        catch (ErrorWithCode exception)
        {
            var errorMessage = _errorMessageProvider.GetString("ScoutingReportStreamRequestFailed") ?? "CB Insights scouting report stream request failed.";
            _logger.LogError(exception, errorMessage + " Code: {Code}", exception.Error);
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = _errorMessageProvider.GetString("ScoutingReportStreamRequestFailed") ?? "CB Insights scouting report stream request failed.";
            _logger.LogError(exception, errorMessage);
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
