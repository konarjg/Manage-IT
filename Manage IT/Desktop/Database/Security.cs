using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using NETCore.Encrypt;
using System.Text;
using NETCore.Encrypt.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;
using System.IO;
using System;
using System.Net.Http;
using System.Windows;

public static class Security
{
    private static string[] EncryptionKey = new string[2];

    public static void Initialize()
    {
        var url = "http://manageit.runasp.net/GetSecurityParameters";
        //var url = "https://localhost:5001/GetSecurityParameters";

        using (var client = new HttpClient())
        {
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            var response = client.Send(message);

            using (var stream = response.Content.ReadAsStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    for (int i = 0; i < EncryptionKey.Length; i++)
                    {
                        EncryptionKey[i] = reader.ReadLine();
                    }
                }
            }
        }
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
