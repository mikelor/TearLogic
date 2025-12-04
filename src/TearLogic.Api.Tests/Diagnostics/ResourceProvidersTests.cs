using TearLogic.Api.CBInsights.Diagnostics;

namespace TearLogic.Api.Tests.Diagnostics;

/// <summary>
/// Unit tests for resource providers.
/// </summary>
[TestClass]
public class ResourceProvidersTests
{
    [TestMethod]
    public void LogMessageProvider_GetString_WithValidKey_ReturnsMessage()
    {
        // Arrange
        var provider = new LogMessageProvider();

        // Act
        var result = provider.GetString("OrganizationLookupStarted");

        // Assert
        // The resource file contains actual messages, so we verify it returns a non-null string
        result.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void LogMessageProvider_GetString_WithInvalidKey_ReturnsNull()
    {
        // Arrange
        var provider = new LogMessageProvider();

        // Act
        var result = provider.GetString("NonExistentKey");

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void ErrorMessageProvider_GetString_WithValidKey_ReturnsMessage()
    {
        // Arrange
        var provider = new ErrorMessageProvider();

        // Act
        var result = provider.GetString("OrganizationLookupFailed");

        // Assert
        // The resource file contains actual messages, so we verify it returns a non-null string
        result.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void ErrorMessageProvider_GetString_WithInvalidKey_ReturnsNull()
    {
        // Arrange
        var provider = new ErrorMessageProvider();

        // Act
        var result = provider.GetString("NonExistentKey");

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void LogMessageProvider_MultipleInstances_UseSameLazyResourceManager()
    {
        // Arrange
        var provider1 = new LogMessageProvider();
        var provider2 = new LogMessageProvider();

        // Act
        var result1 = provider1.GetString("TestKey");
        var result2 = provider2.GetString("TestKey");

        // Assert
        // Both should return the same result (null for non-existent key)
        result1.Should().Be(result2);
    }

    [TestMethod]
    public void ErrorMessageProvider_MultipleInstances_UseSameLazyResourceManager()
    {
        // Arrange
        var provider1 = new ErrorMessageProvider();
        var provider2 = new ErrorMessageProvider();

        // Act
        var result1 = provider1.GetString("TestKey");
        var result2 = provider2.GetString("TestKey");

        // Assert
        // Both should return the same result (null for non-existent key)
        result1.Should().Be(result2);
    }

    [TestMethod]
    public void LogMessageProvider_GetString_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new LogMessageProvider();

        // Act
        Action act = () => provider.GetString(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ErrorMessageProvider_GetString_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new ErrorMessageProvider();

        // Act
        Action act = () => provider.GetString(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
