using FakeItEasy;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Core.Tests;

public class EncryptionServiceInterfaceTests
{
    [Fact]
    public void Encrypt_ShouldReturnEncryptedString()
    {
        // Arrange
        var fakeEncryptionService = A.Fake<IEncryptionService>();
        var plainText = "plainText";
        var encryptedText = "encryptedText";
        var key = "some key";
        A.CallTo(() => fakeEncryptionService.Encrypt(plainText, key)).Returns(encryptedText);

        // Act
        var result = fakeEncryptionService.Encrypt(plainText, key);

        // Assert
        Assert.Equal(encryptedText, result);
    }

    [Fact]
    public void Decrypt_ShouldReturnDecryptedString()
    {
        // Arrange
        var fakeEncryptionService = A.Fake<IEncryptionService>();
        var encryptedText = "encryptedText";
        var decryptedText = "decryptedText";
        var key = "some key";
        A.CallTo(() => fakeEncryptionService.Decrypt(encryptedText, key)).Returns(decryptedText);

        // Act
        var result = fakeEncryptionService.Decrypt(encryptedText, key);

        // Assert
        Assert.Equal(decryptedText, result);
    }
}