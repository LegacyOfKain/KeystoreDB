using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using KeystoreDB.Core.Exceptions;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Services;

public class DatabaseService : IDatabaseService
{
    internal readonly string _dbPath;
    internal readonly object _fileLock = new();
    internal readonly ILogger _logger;
    internal readonly string _password;
    internal ConcurrentDictionary<string, string> _data;
    internal IEncryptionService _encryptionService;
    internal IFileService _fileService;

    // Constructor with only dbPath, password, and ILogger as parameters
    public DatabaseService(string dbPath, string password, ILogger logger)
    {
        _dbPath = dbPath;
        _password = password;
        _logger = logger;

        // Inject IEncryptionService and IFileService using a separate method or property
        InjectDependencies();

        _data = new ConcurrentDictionary<string, string>();
    }

    public DatabaseService(string dbPath, string password, IEncryptionService encryptionService,
        IFileService fileService, ILogger logger)
    {
        _dbPath = dbPath;
        _password = password;
        _encryptionService = encryptionService;
        _fileService = fileService;
        _logger = logger;
        _data = new ConcurrentDictionary<string, string>();
    }

    public void Set(string key, string value)
    {
        _data[key] = value;
        _logger.LogInfo($"Set value for key: {key}");
    }

    public string Get(string key)
    {
        if (_data.TryGetValue(key, out var value))
        {
            _logger.LogInfo($"Retrieved value for key: {key}");
            return value;
        }

        _logger.LogWarning($"Key not found: {key}");
        return null;
    }

    public bool Delete(string key)
    {
        if (_data.TryRemove(key, out _))
        {
            _logger.LogInfo($"Deleted key: {key}");
            return true;
        }

        _logger.LogWarning($"Key not found for deletion: {key}");
        return false;
    }

    public void Save()
    {
        lock (_fileLock)
        {
            try
            {
                var json = JsonSerializer.Serialize(_data);
                var encryptedData = _encryptionService.Encrypt(json, _password);
                _fileService.WriteAllString(_dbPath, encryptedData);
                _logger.LogInfo("Database saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving database", ex);
                throw new KeystoreDBException("Failed to save database", ex);
            }
        }
    }

    public void Load()
    {
        lock (_fileLock)
        {
            try
            {
                _logger.LogInfo($"Attempting to load database from: {_dbPath}");

                if (_fileService.Exists(_dbPath))
                {
                    _logger.LogInfo("Database file found. Loading data...");
                    var encryptedData = _fileService.ReadAllString(_dbPath);
                    _logger.LogInfo($"Read {encryptedData.Length} bytes from file.");

                    try
                    {
                        var json = _encryptionService.Decrypt(encryptedData, _password);
                        _logger.LogInfo("Decryption successful. Attempting to deserialize...");

                        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                        _data = new ConcurrentDictionary<string, string>(data);

                        _logger.LogInfo($"Database loaded successfully. {_data.Count} entries found.");
                    }
                    catch (CryptographicException cryptoEx)
                    {
                        _logger.LogError($"Decryption failed: {cryptoEx.Message}", cryptoEx);
                        _data = new ConcurrentDictionary<string, string>();
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError($"Deserialization failed: {jsonEx.Message}", jsonEx);
                        _data = new ConcurrentDictionary<string, string>();
                    }
                }
                else
                {
                    _logger.LogInfo("No existing database file found. Initializing empty database.");
                    _data = new ConcurrentDictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading database: {ex.Message}", ex);
                _data = new ConcurrentDictionary<string, string>();
            }
        }
    }

    // Separate method to inject dependencies
    private void InjectDependencies()
    {
        _encryptionService = new EncryptionService(); // Replace with actual instantiation
        _fileService = new FileService(); // Replace with actual instantiation
    }
}