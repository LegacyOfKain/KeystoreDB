using System.Text;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Services;

public class FileService : IFileService
{
    private readonly object _fileLock = new();

    public bool Exists(string path)
    {
        lock (_fileLock)
        {
            return File.Exists(path);
        }
    }

    public string ReadAllString(string path)
    {
        lock (_fileLock)
        {
            return File.ReadAllText(path, Encoding.Unicode);
        }
    }

    public void WriteAllString(string path, string text)
    {
        lock (_fileLock)
        {
            File.WriteAllText(path, text, Encoding.Unicode);
        }
    }
}