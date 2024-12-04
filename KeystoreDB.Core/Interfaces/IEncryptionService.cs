namespace KeystoreDB.Core.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText, string password);
    string Decrypt(string cipherText, string password);
}