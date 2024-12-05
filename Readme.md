# KeystoreDB

KeystoreDB is a lightweight, secure NoSQL persistent database engine implemented in C#. It provides a simple key-value
store with encryption support.

## Features

- Simple key-value store
- AES encryption
- Persistent storage
- Thread safe
- Clean Architecture design
- 100% Code Coverage

## Project Structure

The project is organized using Clean Architecture principles:

- `KeystoreDB.Core`: Contains the core business logic and interfaces
- `KeystoreDB`: Main project which implements the interfaces defined in the Core project
- `KeystoreDB.Core.Tests`: Contains unit tests for the Core project
- `KeystoreDB.Tests`: Contains unit tests for the Main project

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio or your preferred IDE
3. Build the solution
4. Run the tests to ensure everything is working correctly

## Usage

Here's a basic example of how to use KeystoreDB:

```csharp
var dbPath = "path/to/database";
var password = "your_secure_password";

var encryptionService = new EncryptionService();
var fileService = new FileService();
var logger = new ConsoleLogger(); // Add this line
var databaseService = new DatabaseService(dbPath, password, logger);
databaseService.Load(); // Load the database from the file if it exists, or initialize an empty database if not found

// Set a value
databaseService.Set("myKey", "myValue");
logger.Log("Value set for myKey");

// Get a value
string value = databaseService.Get("myKey");
logger.Log($"Retrieved value for myKey: {value}");

// Delete a value
bool deleted = databaseService.Delete("myKey");
logger.Log($"Deleted myKey: {deleted}");

// Save changes
databaseService.Save();
logger.Log("Database changes saved");
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
