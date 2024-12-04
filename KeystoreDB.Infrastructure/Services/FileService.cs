using System.Text;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Infrastructure.Services;

public class FileService : IFileService
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public string ReadAllString(string path)
    {
        return File.ReadAllText(path, Encoding.Unicode);
    }

    public void WriteAllString(string path, string text)
    {
        File.WriteAllText(path, text, Encoding.Unicode);
    }
}