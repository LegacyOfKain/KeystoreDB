using KeystoreDB.Core.Interfaces;
using KeystoreDB.Services;

namespace KeystoreDB.Core.Tests;

public class EncryptionServiceTests
{
    private readonly IEncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        _encryptionService = new EncryptionService();
    }

    [Fact]
    public void EncryptAndDecrypt_ShouldReturnOriginalText()
    {
        // Arrange
        var originalText = "This is a secret message";
        var password = "MySecurePassword123!";

        // Act
        var encryptedData = _encryptionService.Encrypt(originalText, password);
        var decryptedText = _encryptionService.Decrypt(encryptedData, password);

        // Assert
        Assert.Equal(originalText, decryptedText);
    }
}