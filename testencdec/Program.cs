using System.Security.Cryptography;
using System.Text;

var code = 24 + "AZp4NY3Wq" + 24 + "lp78tsI5g" + 1 + 24;
var encrypt = Encrypt(code);
var decrypt = Decrypt(encrypt);
Console.WriteLine("Encrypted text : <b>" + encrypt + "</b></br>Decrypted text : <b>" + decrypt + "</b>");


string Decrypt(string cipherText)
{
    var EncryptionKey = "MAKV2SPBNI99212";
    var cipherBytes = Convert.FromBase64String(cipherText);
    using (var encryptor = Aes.Create())
    {
        var pdb = new Rfc2898DeriveBytes(EncryptionKey,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        encryptor.Mode = CipherMode.CBC;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
            }

            cipherText = Encoding.Unicode.GetString(ms.ToArray());
        }
    }

    return cipherText;
}

string Encrypt(string clearText)
{
    var EncryptionKey = "MAKV2SPBNI99212";
    var clearBytes = Encoding.Unicode.GetBytes(clearText);
    using (var encryptor = Aes.Create())
    {
        var pdb = new Rfc2898DeriveBytes(EncryptionKey,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }

            clearText = Convert.ToBase64String(ms.ToArray());
        }
    }

    return clearText;
}