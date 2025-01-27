using NETCore.Encrypt;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

public static class Security
{
    private static string[] EncryptionKey = new string[2];

    public static void Initialize()
    {
        string url = "http://manageit.runasp.net/GetSecurityParameters";

        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = client.Send(message);

            using (Stream stream = response.Content.ReadAsStream())
            {
                using (StreamReader reader = new StreamReader(stream))
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
        byte[] data = encoding.GetBytes(text);
        string result = string.Empty;

        using (SHA512 sha512 = SHA512.Create())
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
