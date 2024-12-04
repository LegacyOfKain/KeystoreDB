using System.Security.Cryptography;
using System.Text.Json;
using KeystoreDB.Core.Exceptions;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _dbPath;
    private readonly IEncryptionService _encryptionService;
    private readonly IFileService _fileService;
    private readonly ILogger _logger;
    private readonly string _password;
    private Dictionary<string, string> _data;

    public DatabaseService(string dbPath, string password, IEncryptionService encryptionService,
        IFileService fileService, ILogger logger)
    {
        _dbPath = dbPath;
        _password = password;
        _encryptionService = encryptionService;
        _fileService = fileService;
        _logger = logger;
        _data = new Dictionary<string, string>();

        Load();
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
        if (_data.Remove(key))
        {
            _logger.LogInfo($"Deleted key: {key}");
            return true;
        }

        _logger.LogWarning($"Key not found for deletion: {key}");
        return false;
    }

    public void Save()
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

    private void Load()
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

                    _data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    _logger.LogInfo($"Database loaded successfully. {_data.Count} entries found.");
                }
                catch (CryptographicException cryptoEx)
                {
                    _logger.LogError($"Decryption failed: {cryptoEx.Message}", cryptoEx);
                    _data = new Dictionary<string, string>();
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError($"Deserialization failed: {jsonEx.Message}", jsonEx);
                    _data = new Dictionary<string, string>();
                }
            }
            else
            {
                _logger.LogInfo("No existing database file found. Initializing empty database.");
                _data = new Dictionary<string, string>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading database: {ex.Message}", ex);
            _data = new Dictionary<string, string>();
        }
    }
}