using TearLogic.Api.CBInsights.Configuration;
using TearLogic.Api.CBInsights.Diagnostics;
using TearLogic.Api.CBInsights.Infrastructure;
using TearLogic.Clients.Models.V2OrganizationLookup;

namespace TearLogic.Api.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="CBInsightsClient"/>.
/// </summary>
[TestClass]
public class CBInsightsClientTests
{
    private Mock<ICBInsightsRequestAdapterFactory> _mockAdapterFactory = null!;
    private Mock<IOptionsMonitor<CBInsightsOptions>> _mockOptionsMonitor = null!;
    private Mock<ILogger<CBInsightsClient>> _mockLogger = null!;
    private Mock<IErrorMessageProvider> _mockErrorMessageProvider = null!;
    private Mock<ILogMessageProvider> _mockLogMessageProvider = null!;
    private CBInsightsOptions _options = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAdapterFactory = new Mock<ICBInsightsRequestAdapterFactory>();
        _mockOptionsMonitor = new Mock<IOptionsMonitor<CBInsightsOptions>>();
        _mockLogger = new Mock<ILogger<CBInsightsClient>>();
        _mockErrorMessageProvider = new Mock<IErrorMessageProvider>();
        _mockLogMessageProvider = new Mock<ILogMessageProvider>();

        _options = new CBInsightsOptions
        {
            BaseUrl = "https://api.cbinsights.com",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            TokenCacheDurationMinutes = 55
        };

        _mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(_options);
    }

    [TestMethod]
    public void Constructor_WithNullRequestAdapterFactory_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsClient(
            null!,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            _mockErrorMessageProvider.Object,
            _mockLogMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("requestAdapterFactory");
    }

    [TestMethod]
    public void Constructor_WithNullOptionsMonitor_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsClient(
            _mockAdapterFactory.Object,
            null!,
            _mockLogger.Object,
            _mockErrorMessageProvider.Object,
            _mockLogMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("optionsMonitor");
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsClient(
            _mockAdapterFactory.Object,
            _mockOptionsMonitor.Object,
            null!,
            _mockErrorMessageProvider.Object,
            _mockLogMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [TestMethod]
    public void Constructor_WithNullErrorMessageProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsClient(
            _mockAdapterFactory.Object,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            null!,
            _mockLogMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("errorMessageProvider");
    }

    [TestMethod]
    public void Constructor_WithNullLogMessageProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsClient(
            _mockAdapterFactory.Object,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            _mockErrorMessageProvider.Object,
            null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logMessageProvider");
    }

    [TestMethod]
    public async Task LookupOrganizationAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.LookupOrganizationAsync(
            null!, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task GetFirmographicsAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.GetFirmographicsAsync(
            null!, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task GetFundingsAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();
        var request = new TearLogic.Clients.Models.V2FinancialTransactions.ListTransactionsForOrganizationRequest();

        // Act
        Func<Task> act = async () => await client.GetFundingsAsync(
            0, 
            request, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetFundingsAsync_WithNegativeOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();
        var request = new TearLogic.Clients.Models.V2FinancialTransactions.ListTransactionsForOrganizationRequest();

        // Act
        Func<Task> act = async () => await client.GetFundingsAsync(
            -1, 
            request, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetInvestmentsAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();
        var request = new TearLogic.Clients.Models.V2FinancialTransactions.ListTransactionsForOrganizationRequest();

        // Act
        Func<Task> act = async () => await client.GetInvestmentsAsync(
            0, 
            request, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetPortfolioExitsAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.GetPortfolioExitsAsync(
            0, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetBusinessRelationshipsAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.GetBusinessRelationshipsAsync(
            0, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetManagementAndBoardAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();
        var request = new TearLogic.Clients.Models.V2ManagementAndBoard.ManagementAndBoardRequestBody();

        // Act
        Func<Task> act = async () => await client.GetManagementAndBoardAsync(
            0, 
            request, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetOutlookAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.GetOutlookAsync(
            0, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task GetScoutingReportAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.GetScoutingReportAsync(
            0, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task StreamScoutingReportAsync_WithInvalidOrganizationId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.StreamScoutingReportAsync(
            0, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("organizationId");
    }

    [TestMethod]
    public async Task SendChatCbiRequestAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.SendChatCbiRequestAsync(
            null!, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task StreamChatCbiAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var client = CreateClient();

        // Act
        Func<Task> act = async () => await client.StreamChatCbiAsync(
            null!, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    private CBInsightsClient CreateClient()
    {
        return new CBInsightsClient(
            _mockAdapterFactory.Object,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            _mockErrorMessageProvider.Object,
            _mockLogMessageProvider.Object);
    }
}
