namespace KeystoreDB.Core.Interfaces;

public interface IFileService
{
    void WriteAllString(string path, string text);
    string ReadAllString(string path);
    bool Exists(string path);
}