using System.Net;
using TearLogic.Api.CBInsights.Configuration;
using TearLogic.Api.CBInsights.Diagnostics;
using TearLogic.Api.CBInsights.Infrastructure;

namespace TearLogic.Api.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="CBInsightsTokenProvider"/>.
/// </summary>
[TestClass]
public class CBInsightsTokenProviderTests
{
    private Mock<IMemoryCache> _mockMemoryCache = null!;
    private Mock<IHttpClientFactory> _mockHttpClientFactory = null!;
    private Mock<IOptionsMonitor<CBInsightsOptions>> _mockOptionsMonitor = null!;
    private Mock<ILogger<CBInsightsTokenProvider>> _mockLogger = null!;
    private Mock<ILogMessageProvider> _mockLogMessageProvider = null!;
    private Mock<IErrorMessageProvider> _mockErrorMessageProvider = null!;
    private CBInsightsOptions _options = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockOptionsMonitor = new Mock<IOptionsMonitor<CBInsightsOptions>>();
        _mockLogger = new Mock<ILogger<CBInsightsTokenProvider>>();
        _mockLogMessageProvider = new Mock<ILogMessageProvider>();
        _mockErrorMessageProvider = new Mock<IErrorMessageProvider>();

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
    public void Constructor_WithNullMemoryCache_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsTokenProvider(
            null!,
            _mockHttpClientFactory.Object,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            _mockLogMessageProvider.Object,
            _mockErrorMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("memoryCache");
    }

    [TestMethod]
    public void Constructor_WithNullHttpClientFactory_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsTokenProvider(
            _mockMemoryCache.Object,
            null!,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            _mockLogMessageProvider.Object,
            _mockErrorMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("httpClientFactory");
    }

    [TestMethod]
    public void Constructor_WithNullOptionsMonitor_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsTokenProvider(
            _mockMemoryCache.Object,
            _mockHttpClientFactory.Object,
            null!,
            _mockLogger.Object,
            _mockLogMessageProvider.Object,
            _mockErrorMessageProvider.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("optionsMonitor");
    }

    [TestMethod]
    public async Task GetTokenAsync_WhenTokenInCache_ReturnsCachedToken()
    {
        // Arrange
        const string cachedToken = "cached-token-123";
        object? cacheValue = cachedToken;
        
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(true);

        var provider = CreateProvider();

        // Act
        var result = await provider.GetTokenAsync(CancellationToken.None);

        // Assert
        result.Should().Be(cachedToken);
        _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task GetTokenAsync_WhenTokenNotInCache_CallsHttpClientFactory()
    {
        // Arrange
        object? cacheValue = null;
        
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(false);

        var mockHttpMessageHandler = new MockHttpMessageHandler(
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token\":\"new-token-456\"}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri(_options.BaseUrl)
        };

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(CBInsightsHttpClientNames.Authorization))
            .Returns(httpClient);

        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());

        var provider = CreateProvider();

        // Act
        var result = await provider.GetTokenAsync(CancellationToken.None);

        // Assert
        result.Should().Be("new-token-456");
        _mockHttpClientFactory.Verify(
            x => x.CreateClient(CBInsightsHttpClientNames.Authorization), 
            Times.Once);
    }

    [TestMethod]
    public async Task GetTokenAsync_WhenHttpRequestFails_ThrowsInvalidOperationException()
    {
        // Arrange
        object? cacheValue = null;
        
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(false);

        var mockHttpMessageHandler = new MockHttpMessageHandler(
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            });

        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri(_options.BaseUrl)
        };

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(CBInsightsHttpClientNames.Authorization))
            .Returns(httpClient);

        var provider = CreateProvider();

        // Act
        Func<Task> act = async () => await provider.GetTokenAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*authorization token*");
    }

    [TestMethod]
    public async Task GetTokenAsync_WhenResponseMissingToken_ThrowsInvalidOperationException()
    {
        // Arrange
        object? cacheValue = null;
        
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(false);

        var mockHttpMessageHandler = new MockHttpMessageHandler(
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri(_options.BaseUrl)
        };

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(CBInsightsHttpClientNames.Authorization))
            .Returns(httpClient);

        var provider = CreateProvider();

        // Act
        Func<Task> act = async () => await provider.GetTokenAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*did not include a token*");
    }

    private CBInsightsTokenProvider CreateProvider()
    {
        return new CBInsightsTokenProvider(
            _mockMemoryCache.Object,
            _mockHttpClientFactory.Object,
            _mockOptionsMonitor.Object,
            _mockLogger.Object,
            _mockLogMessageProvider.Object,
            _mockErrorMessageProvider.Object);
    }

    /// <summary>
    /// Simple mock HTTP message handler for testing.
    /// </summary>
    private class MockHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response = response;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}
