using FluentAssertions;
using KeystoreDB.Core.Exceptions;

namespace KeystoreDB.Core.Tests;

public class KeystoreDBExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateExceptionWithNoMessageAndNoInnerException()
    {
        // Act
        var exception = new KeystoreDBException();

        // Assert
        exception.Message.Should().Be("Exception of type 'KeystoreDB.Core.Exceptions.KeystoreDBException' was thrown.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void ConstructorWithMessage_ShouldCreateExceptionWithSpecifiedMessage()
    {
        // Arrange
        var message = "Test exception message";

        // Act
        var exception = new KeystoreDBException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void ConstructorWithMessageAndInnerException_ShouldCreateExceptionWithSpecifiedMessageAndInnerException()
    {
        // Arrange
        var message = "Test exception message";
        var innerException = new ArgumentException("Inner exception message");

        // Act
        var exception = new KeystoreDBException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeOfType<ArgumentException>()
            .Which.Message.Should().Be("Inner exception message");
    }

    [Fact]
    public void KeystoreDBException_ShouldInheritFromException()
    {
        // Act
        var exception = new KeystoreDBException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}