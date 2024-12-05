namespace KeystoreDB.Core.Interfaces;

public interface IDatabaseService
{
    void Set(string key, string value);
    string Get(string key);
    bool Delete(string key);
    void Save(); // Add this line

    void Load(); // Load the database from the file if it exists, or initialize an empty database if not found  
}