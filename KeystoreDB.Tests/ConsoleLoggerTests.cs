using FluentAssertions;
using KeystoreDB.Services;

namespace KeystoreDB.Core.Tests;

public class ConsoleLoggerTests
{
    [Fact]
    public void LogError_ShouldWriteFormattedErrorMessageToConsole()
    {
        // Arrange
        var consoleLogger = new ConsoleLogger();
        var errorMessage = "Test error message";
        var testException = new Exception("Test exception");

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        consoleLogger.LogError(errorMessage, testException);

        // Assert
        var output = consoleOutput.ToString();
        output.Should().Contain("[ERROR]");
        output.Should().Contain(errorMessage);
        output.Should().Contain("Exception:");
        output.Should().Contain(testException.Message);
    }

    [Fact]
    public void LogError_WithoutException_ShouldWriteFormattedErrorMessageToConsole()
    {
        // Arrange
        var consoleLogger = new ConsoleLogger();
        var errorMessage = "Test error message";

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        consoleLogger.LogError(errorMessage);

        // Assert
        var output = consoleOutput.ToString();
        output.Should().Contain("[ERROR]");
        output.Should().Contain(errorMessage);
        output.Should().NotContain("Exception:");
    }
}