using KeystoreDB.Core.Interfaces;
using KeystoreDB.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KeystoreDBProgram;

public class Program
{
    private static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        var database = serviceProvider.GetRequiredService<IDatabaseService>();

        // Example usage
        Console.WriteLine(database.Get("key1")); // Outputs: value1
        database.Set("key1", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        Console.WriteLine(database.Get("key1")); // Outputs: value1
        database.Set("key2", "gh");
        database.Delete("key2");
        Console.WriteLine(database.Get("key2")); // Outputs: null

        // Save the changes to the database file
        database.Save();
        Console.WriteLine("Database saved successfully.");
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Add your custom logger implementation
        services.AddSingleton<ILogger, ConsoleLogger>(); // Assume you have a ConsoleLogger class

        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IDatabaseService>(sp => new DatabaseService(
            @"C:/temp/database.dat",
            "secret123",
            sp.GetRequiredService<IEncryptionService>(),
            sp.GetRequiredService<IFileService>(),
            sp.GetRequiredService<ILogger>()
        ));
    }
}