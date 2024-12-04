using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Services;

public class ConsoleLogger : ILogger
{
    private readonly object _consoleLock = new();

    public void LogInfo(string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }
    }

    public void LogWarning(string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] {message}");
            Console.ResetColor();
        }
    }

    public void LogError(string message, Exception exception = null)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine($"StackTrace: {exception.StackTrace}");
            }

            Console.ResetColor();
        }
    }
}