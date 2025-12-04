using System.ComponentModel.DataAnnotations;
using TearLogic.Api.CBInsights.Configuration;

namespace TearLogic.Api.Tests.Configuration;

/// <summary>
/// Unit tests for <see cref="CBInsightsOptions"/>.
/// </summary>
[TestClass]
public class CBInsightsOptionsTests
{
    [TestMethod]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = "https://api.cbinsights.com",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            TokenCacheDurationMinutes = 55
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [TestMethod]
    public void Validate_WithMissingBaseUrl_ReturnsValidationError()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = string.Empty,
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CBInsightsOptions.BaseUrl)));
    }

    [TestMethod]
    public void Validate_WithInvalidUrl_ReturnsValidationError()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = "not-a-valid-url",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CBInsightsOptions.BaseUrl)));
    }

    [TestMethod]
    public void Validate_WithMissingClientId_ReturnsValidationError()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = "https://api.cbinsights.com",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = string.Empty,
            ClientSecret = "test-client-secret"
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CBInsightsOptions.ClientId)));
    }

    [TestMethod]
    public void Validate_WithMissingClientSecret_ReturnsValidationError()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = "https://api.cbinsights.com",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = string.Empty
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CBInsightsOptions.ClientSecret)));
    }

    [TestMethod]
    public void Validate_WithTokenCacheDurationBelowMinimum_ReturnsValidationError()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = "https://api.cbinsights.com",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            TokenCacheDurationMinutes = 0
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CBInsightsOptions.TokenCacheDurationMinutes)));
    }

    [TestMethod]
    public void Validate_WithTokenCacheDurationAboveMaximum_ReturnsValidationError()
    {
        // Arrange
        var options = new CBInsightsOptions
        {
            BaseUrl = "https://api.cbinsights.com",
            AuthorizeEndpoint = "/auth/token",
            OrganizationsEndpoint = "/v2/organizations",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            TokenCacheDurationMinutes = 300
        };

        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains(nameof(CBInsightsOptions.TokenCacheDurationMinutes)));
    }

    [TestMethod]
    public void DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var options = new CBInsightsOptions();

        // Assert
        options.BaseUrl.Should().Be(string.Empty);
        options.AuthorizeEndpoint.Should().Be(string.Empty);
        options.OrganizationsEndpoint.Should().Be(string.Empty);
        options.ClientId.Should().Be(string.Empty);
        options.ClientSecret.Should().Be(string.Empty);
        options.TokenCacheDurationMinutes.Should().Be(55);
    }
}
