using KeyValuePair = KeystoreDB.Core.Entities.KeyValuePair;

namespace KeystoreDB.Core.Tests;

public class KeyValuePairTests
{
    [Fact]
    public void KeyValuePair_ShouldStoreKeyAndValue()
    {
        // Arrange
        var keyValuePair = new KeyValuePair();

        // Act
        keyValuePair.Key = "TestKey";
        keyValuePair.Value = "TestValue";

        // Assert
        Assert.Equal("TestKey", keyValuePair.Key);
        Assert.Equal("TestValue", keyValuePair.Value);
    }

    [Fact]
    public void KeyValuePair_ShouldAllowEmptyKeyAndValue()
    {
        // Arrange
        var keyValuePair = new KeyValuePair();

        // Act
        keyValuePair.Key = string.Empty;
        keyValuePair.Value = string.Empty;

        // Assert
        Assert.Equal(string.Empty, keyValuePair.Key);
        Assert.Equal(string.Empty, keyValuePair.Value);
    }

    [Fact]
    public void KeyValuePair_ShouldAllowNullKeyAndValue()
    {
        // Arrange
        var keyValuePair = new KeyValuePair();

        // Act
        keyValuePair.Key = null;
        keyValuePair.Value = null;

        // Assert
        Assert.Null(keyValuePair.Key);
        Assert.Null(keyValuePair.Value);
    }
}