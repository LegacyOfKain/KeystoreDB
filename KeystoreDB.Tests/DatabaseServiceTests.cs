using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using FakeItEasy;
using FluentAssertions;
using KeystoreDB.Core.Exceptions;
using KeystoreDB.Core.Interfaces;
using KeystoreDB.Services;

namespace KeystoreDB.Core.Tests;

public class DatabaseServiceTests : IDisposable
{
    private const string Password = "password@123";
    private readonly IDatabaseService _database;
    private readonly IEncryptionService _encryptionService;
    private readonly IFileService _fileService;
    private readonly ILogger _logger;
    private readonly string _tempDbPath;

    public DatabaseServiceTests()
    {
        _tempDbPath = Path.GetTempFileName();
        _encryptionService = new EncryptionService();
        _fileService = new FileService();
        _logger = new ConsoleLogger();

        _database = new DatabaseService(
            _tempDbPath,
            Password,
            _encryptionService,
            _fileService,
            _logger
        );
    }

    public void Dispose()
    {
        if (File.Exists(_tempDbPath)) File.Delete(_tempDbPath);
    }

    [Fact]
    public void SetAndGet_ShouldReturnSameValue()
    {
        // Arrange
        const string key = "key1";
        const string value = "value1";

        // Act
        _database.Set(key, value);
        var retrievedValue = _database.Get(key);

        // Assert
        Assert.Equal(value, retrievedValue);
    }

    [Fact]
    public void DeleteAfterSet_ShouldNotReturnSameValue()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";
        _database.Set(key, value);

        // Act
        var deleteResult = _database.Delete(key);
        var retrievedValue = _database.Get(key);

        // Assert
        Assert.True(deleteResult);
        Assert.Null(retrievedValue);
    }

    [Fact]
    public void Load_WhenFileExistsAndIsValid_ShouldLoadDataSuccessfully()
    {
        // Arrange
        var testData = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
        var json = JsonSerializer.Serialize(testData);
        var encryptedData = _encryptionService.Encrypt(json, Password);
        _fileService.WriteAllString(_tempDbPath, encryptedData);

        var databaseService = new DatabaseService(_tempDbPath, Password, _encryptionService, _fileService, _logger);

        // Act
        databaseService.Load();

        // Assert
        Assert.Equal("value1", databaseService.Get("key1"));
        Assert.Equal("value2", databaseService.Get("key2"));
    }

    [Fact]
    public void Load_WhenLoadingCorruptInvalidFile_ShouldThrowException()
    {
        // Arrange
        _fileService.WriteAllString(_tempDbPath, "This is not valid encrypted data");

        var mockLogger = new MockLogger();
        var databaseService = new DatabaseService(
            _tempDbPath,
            "test_password",
            _encryptionService,
            _fileService,
            mockLogger
        );

        // Act
        databaseService.Load();

        // Assert
        Assert.Contains(mockLogger.LoggedErrors, error => error.StartsWith("Error loading database:"));
    }

    [Fact]
    public void Load_WhenDecryptionFails_ShouldLogErrorAndInitializeEmptyDatabase()
    {
        // Arrange
        var mockLogger = new MockLogger();
        var testData = new Dictionary<string, string> { { "key1", "value1" } };
        var json = JsonSerializer.Serialize(testData);
        var encryptedData = _encryptionService.Encrypt(json, Password);
        _fileService.WriteAllString(_tempDbPath, encryptedData);

        var databaseService = new DatabaseService(
            _tempDbPath,
            Password + "123", // Wrong password
            _encryptionService,
            _fileService,
            mockLogger
        );

        // Act
        databaseService.Load();

        // Assert
        Assert.Contains(mockLogger.LoggedErrors, error => error.StartsWith("Decryption failed:"));
        Assert.Null(databaseService.Get("key1")); // Verify that the database is empty
    }

    [Fact]
    public void Load_WhenDatabaseFileDoesNotExist_ShouldLogErrorAndInitializeEmptyDatabase()
    {
        // Arrange
        var mockFileService = A.Fake<IFileService>();
        var mockLogger = A.Fake<ILogger>();
        var mockEncryptionService = A.Fake<IEncryptionService>();

        A.CallTo(() => mockFileService.Exists(A<string>._)).Returns(false);

        var nonExistentDbPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var databaseService = new DatabaseService(
            nonExistentDbPath,
            Password,
            mockEncryptionService,
            mockFileService,
            mockLogger
        );

        // Act
        databaseService.Load();

        // Assert
        A.CallTo(() => mockLogger.LogInfo("No existing database file found. Initializing empty database."))
            .MustHaveHappened();

        Assert.Null(databaseService.Get("any_key"));
    }

    [Fact]
    public void Save_ShouldEncryptAndWriteDataToFile()
    {
        // Arrange
        var mockFileService = A.Fake<IFileService>();
        var mockEncryptionService = A.Fake<IEncryptionService>();
        var mockLogger = A.Fake<ILogger>();

        var testData = new ConcurrentDictionary<string, string>();
        testData["key1"] = "value1";
        testData["key2"] = "value2";

        var databaseService = new DatabaseService(
            "testPath",
            "testPassword",
            mockEncryptionService,
            mockFileService,
            mockLogger
        );

        // Use reflection to set the private _data field
        typeof(DatabaseService).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(databaseService, testData);

        var expectedJson = JsonSerializer.Serialize(testData);
        var expectedEncryptedData = "encryptedData";

        A.CallTo(() => mockEncryptionService.Encrypt(expectedJson, "testPassword"))
            .Returns(expectedEncryptedData);

        // Act
        var act = () => databaseService.Save();

        // Assert
        act.Should().NotThrow();

        A.CallTo(() => mockEncryptionService.Encrypt(expectedJson, "testPassword"))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockFileService.WriteAllString("testPath", expectedEncryptedData))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockLogger.LogInfo("Database saved successfully"))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Save_WhenExceptionOccurs_ShouldLogErrorAndThrowKeystoreDBException()
    {
        // Arrange
        var mockFileService = A.Fake<IFileService>();
        var mockEncryptionService = A.Fake<IEncryptionService>();
        var mockLogger = A.Fake<ILogger>();

        var databaseService = new DatabaseService(
            "testPath",
            "testPassword",
            mockEncryptionService,
            mockFileService,
            mockLogger
        );

        A.CallTo(() => mockEncryptionService.Encrypt(A<string>._, A<string>._))
            .Throws(new Exception("Encryption failed"));

        // Act
        var act = () => databaseService.Save();

        // Assert
        act.Should().Throw<KeystoreDBException>()
            .WithMessage("Failed to save database")
            .WithInnerException<Exception>()
            .WithMessage("Encryption failed");

        A.CallTo(() => mockLogger.LogError("Error saving database", A<Exception>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Delete_WhenKeyExists_ShouldRemoveKeyAndReturnTrue()
    {
        // Arrange
        var mockLogger = A.Fake<ILogger>();
        var testData = new ConcurrentDictionary<string, string>();
        testData["existingKey"] = "someValue";

        var databaseService = new DatabaseService(
            "testPath",
            "testPassword",
            A.Fake<IEncryptionService>(),
            A.Fake<IFileService>(),
            mockLogger
        );

        databaseService.Set("existingKey", "someValue");

        // Act
        var result = databaseService.Delete("existingKey");

        // Assert
        result.Should().BeTrue();
        A.CallTo(() => mockLogger.LogInfo("Deleted key: existingKey")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Delete_WhenKeyDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var mockLogger = A.Fake<ILogger>();
        var testData = new ConcurrentDictionary<string, string>();

        var databaseService = new DatabaseService(
            "testPath",
            "testPassword",
            A.Fake<IEncryptionService>(),
            A.Fake<IFileService>(),
            mockLogger
        );

        // Act
        var result = databaseService.Delete("nonExistentKey");

        // Assert
        result.Should().BeFalse();
        A.CallTo(() => mockLogger.LogWarning("Key not found for deletion: nonExistentKey"))
            .MustHaveHappenedOnceExactly();
    }

    // Mock logger class for testing
    private class MockLogger : ILogger
    {
        public List<string> LoggedErrors { get; } = new();

        public void LogInfo(string message)
        {
        }

        public void LogWarning(string message)
        {
        }

        public void LogError(string message, Exception exception = null)
        {
            LoggedErrors.Add(message);
        }

        public void LogError(string message)
        {
            LoggedErrors.Add(message);
        }
    }
}