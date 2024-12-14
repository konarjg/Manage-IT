using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using NETCore.Encrypt;
using System.Text;
using NETCore.Encrypt.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Server;
using System.Web;

public static class Security
{
    private static string[] EncryptionKey = new string[2];

    public static void Initialize()
    {
        var path = System.AppDomain.CurrentDomain.BaseDirectory + "/scr.cfg";

        if (File.Exists(path))
        {
            using (var file = File.OpenRead(path))
            {
                int i = 0;

                using (var reader = new StreamReader(file))
                {
                    var line = reader.ReadLine();

                    EncryptionKey[i] = line == null ? string.Empty : line;
                    i++;
                }
            }

            return;
        }

        var aes = EncryptProvider.CreateAesKey();
        EncryptionKey[0] = aes.Key;
        EncryptionKey[1] = aes.IV;

        var content = aes.Key + "\n" + aes.IV;
        File.WriteAllText(path, content);
    }

    public static string HashText(string text, Encoding encoding)
    {
        var data = encoding.GetBytes(text);
        string result = string.Empty;

        using (var sha512 = SHA512.Create())
        {
            result = encoding.GetString(sha512.ComputeHash(data));
        }

        return result;
    }

    public static string EncryptText(string text)
    {
        return EncryptProvider.AESEncrypt(text, EncryptionKey[0], EncryptionKey[1]);
    }

    public static string DecryptText(string text)
    {
        return EncryptProvider.AESDecrypt(text, EncryptionKey[0], EncryptionKey[1]);
    }
}
