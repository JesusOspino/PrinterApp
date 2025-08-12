using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NovaPrinter.Shared;

public static class Utils
{
    private static readonly byte[] FixedIV = new byte[16] {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };

    private static string _KeyValue = "MyLLaveSecret";

    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(_KeyValue);
        aes.IV = FixedIV;

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(Encoding.UTF8.GetBytes(plainText));
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static string Decrypt(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(_KeyValue);
        aes.IV = FixedIV;

        using var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText));
        using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }

    private static byte[] DeriveKey(string secretKey)
    {
        // Usamos SHA256 para derivar una clave de tamaño fijo a partir de la llave secreta
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
    }
}
