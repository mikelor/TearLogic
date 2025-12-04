using TearLogic.Api.CBInsights.Infrastructure;

namespace TearLogic.Api.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="CBInsightsRequestAdapterFactory"/>.
/// </summary>
[TestClass]
public class CBInsightsRequestAdapterFactoryTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory = null!;
    private Mock<ICBInsightsAuthenticationProvider> _mockAuthProvider = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockAuthProvider = new Mock<ICBInsightsAuthenticationProvider>();
    }

    [TestMethod]
    public void Constructor_WithNullHttpClientFactory_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsRequestAdapterFactory(
            null!,
            _mockAuthProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClientFactory");
    }

    [TestMethod]
    public void Constructor_WithNullAuthenticationProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsRequestAdapterFactory(
            _mockHttpClientFactory.Object,
            null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("authenticationProvider");
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsRequestAdapter()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.cbinsights.com")
        };

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(CBInsightsHttpClientNames.Api))
            .Returns(httpClient);

        var factory = new CBInsightsRequestAdapterFactory(
            _mockHttpClientFactory.Object,
            _mockAuthProvider.Object);

        // Act
        var adapter = await factory.CreateAsync(CancellationToken.None);

        // Assert
        adapter.Should().NotBeNull();
        _mockHttpClientFactory.Verify(
            x => x.CreateClient(CBInsightsHttpClientNames.Api), 
            Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var factory = new CBInsightsRequestAdapterFactory(
            _mockHttpClientFactory.Object,
            _mockAuthProvider.Object);

        // Act
        Func<Task> act = async () => await factory.CreateAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
