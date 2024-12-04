# KeystoreDB

KeystoreDB is a lightweight, secure NoSQL persistent database engine implemented in C#. It provides a simple key-value store with encryption support using AES-CBC with PKCS7 padding.

## Features

- Simple key-value store
- AES-CBC encryption with PKCS7 padding
- Persistent storage
- Thread safe
- Clean Architecture design

## Project Structure

The project is organized using Clean Architecture principles:

- `KeystoreDB.Core`: Contains the core business logic and interfaces
- `KeystoreDB.Infrastructure`: Implements the interfaces defined in the Core project
- `KeystoreDB.Tests`: Contains unit tests, integration tests,
- `KeystoreDB.Core.Tests`: 

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
var databaseService = new DatabaseService(dbPath, password, encryptionService, fileService);

// Set a value
databaseService.Set("myKey", "myValue");

// Get a value
string value = databaseService.Get("myKey");

// Delete a value
bool deleted = databaseService.Delete("myKey");

// Save changes
databaseService.Save();
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.