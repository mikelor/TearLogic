using TearLogic.Api.CBInsights.Commands;

namespace TearLogic.Api.Tests.Commands;

/// <summary>
/// Unit tests for <see cref="CommandHandler{TCommand, TResult}"/>.
/// </summary>
[TestClass]
public class CommandHandlerTests
{
    [TestMethod]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new TestCommandHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [TestMethod]
    public void Constructor_WithValidLogger_CreatesHandler()
    {
        // Arrange
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TestCommandHandler>();

        // Act
        var handler = new TestCommandHandler(logger);

        // Assert
        handler.Should().NotBeNull();
    }

    [TestMethod]
    public async Task HandleAsync_CallsAbstractMethod()
    {
        // Arrange
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TestCommandHandler>();
        var handler = new TestCommandHandler(logger);
        var command = new TestCommand { Value = "test" };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Processed: test");
    }

    // Test implementation classes
    public class TestCommand
    {
        public string Value { get; set; } = string.Empty;
    }

    public class TestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class TestCommandHandler(ILogger<TestCommandHandler> logger) 
        : CommandHandler<TestCommand, TestResult>(logger)
    {
        public override Task<TestResult> HandleAsync(TestCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            
            var result = new TestResult
            {
                Success = true,
                Message = $"Processed: {command.Value}"
            };

            return Task.FromResult(result);
        }
    }
}
