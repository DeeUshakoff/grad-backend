using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Helpers;
public interface IEncryptionService
{
    string Encrypt(string data);
    string Decrypt(string encryptedData);
}
public class EncryptionOptions
{
    public string GitEncryptionKey { get; set; }
    public string GitEncryptionIV { get; set; }
}

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IOptions<EncryptionOptions> options)
    {
        var encryptionOptions = options.Value;
        _key = Convert.FromBase64String(encryptionOptions.GitEncryptionKey);
        _iv = Convert.FromBase64String(encryptionOptions.GitEncryptionIV);
    }

    public string Encrypt(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = EncryptInternal(plainTextBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    private byte[] EncryptInternal(byte[] data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();

            return memoryStream.ToArray();
        }
    }

    public string Decrypt(string encryptedText)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] decryptedBytes = DecryptInternal(encryptedBytes);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private byte[] DecryptInternal(byte[] encryptedData)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(encryptedData);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using MemoryStream outputStream = new();
            cryptoStream.CopyTo(outputStream);

            return outputStream.ToArray();
        }
    }
}