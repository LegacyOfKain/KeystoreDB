namespace KeystoreDB.Core.Interfaces;

public interface IDatabaseService
{
    void Set(string key, string value);
    string Get(string key);
    bool Delete(string key);
    void Save(); // Add this line
}