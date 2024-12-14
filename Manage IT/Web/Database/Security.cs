using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using NETCore.Encrypt;
using System.Text;
using NETCore.Encrypt.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;
using System.IO;
using System;

public static class Security
{
    private static string[] EncryptionKey = new string[2];

    public static string Parameters
    {
        get
        {
            var parameters = string.Empty;
            parameters += EncryptionKey[0] + "\n";
            parameters += EncryptionKey[1];

            return parameters;
        }
    }

    public static void Initialize()
    {
        var path = System.AppDomain.CurrentDomain.BaseDirectory + "/scr.cfg";

        if (File.Exists(path))
        {
            var lines = File.ReadAllLines(path);
            
            for (int i = 0; i < EncryptionKey.Length; i++)
            {
                EncryptionKey[i] = lines[i];
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
            result = Convert.ToBase64String(sha512.ComputeHash(data));
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
