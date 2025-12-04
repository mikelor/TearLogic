using Microsoft.Kiota.Abstractions;
using TearLogic.Api.CBInsights.Infrastructure;

namespace TearLogic.Api.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="CBInsightsAuthenticationProvider"/>.
/// </summary>
[TestClass]
public class CBInsightsAuthenticationProviderTests
{
    private Mock<ICBInsightsTokenProvider> _mockTokenProvider = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockTokenProvider = new Mock<ICBInsightsTokenProvider>();
    }

    [TestMethod]
    public void Constructor_WithNullTokenProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new CBInsightsAuthenticationProvider(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("tokenProvider");
    }

    [TestMethod]
    public async Task AuthenticateRequestAsync_WithValidToken_AddsAuthorizationHeader()
    {
        // Arrange
        const string expectedToken = "test-token-123";
        _mockTokenProvider
            .Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedToken);

        var provider = new CBInsightsAuthenticationProvider(_mockTokenProvider.Object);
        var requestInfo = new RequestInformation
        {
            HttpMethod = Method.GET,
            UrlTemplate = "https://api.cbinsights.com/test"
        };

        // Act
        await provider.AuthenticateRequestAsync(requestInfo, null, CancellationToken.None);

        // Assert
        requestInfo.Headers.Should().ContainKey("Authorization");
        requestInfo.Headers["Authorization"].Should().ContainSingle()
            .Which.Should().Be($"Bearer {expectedToken}");
    }

    [TestMethod]
    public async Task AuthenticateRequestAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new CBInsightsAuthenticationProvider(_mockTokenProvider.Object);

        // Act
        Func<Task> act = async () => await provider.AuthenticateRequestAsync(
            null!, 
            null, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task AuthenticateRequestAsync_WhenTokenProviderThrows_PropagatesException()
    {
        // Arrange
        _mockTokenProvider
            .Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Token acquisition failed"));

        var provider = new CBInsightsAuthenticationProvider(_mockTokenProvider.Object);
        var requestInfo = new RequestInformation
        {
            HttpMethod = Method.GET,
            UrlTemplate = "https://api.cbinsights.com/test"
        };

        // Act
        Func<Task> act = async () => await provider.AuthenticateRequestAsync(
            requestInfo, 
            null, 
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Token acquisition failed");
    }

    [TestMethod]
    public async Task AuthenticateRequestAsync_WithCancellationToken_PassesToTokenProvider()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        
        _mockTokenProvider
            .Setup(x => x.GetTokenAsync(cancellationToken))
            .ReturnsAsync("test-token");

        var provider = new CBInsightsAuthenticationProvider(_mockTokenProvider.Object);
        var requestInfo = new RequestInformation
        {
            HttpMethod = Method.GET,
            UrlTemplate = "https://api.cbinsights.com/test"
        };

        // Act
        await provider.AuthenticateRequestAsync(requestInfo, null, cancellationToken);

        // Assert
        _mockTokenProvider.Verify(x => x.GetTokenAsync(cancellationToken), Times.Once);
    }
}
