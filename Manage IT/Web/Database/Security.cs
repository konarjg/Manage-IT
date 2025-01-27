using NETCore.Encrypt;
using System.Security.Cryptography;
using System.Text;

public static class Security
{
    private static string[] EncryptionKey = new string[2];

    public static string Parameters
    {
        get
        {
            string parameters = string.Empty;
            parameters += EncryptionKey[0] + "\n";
            parameters += EncryptionKey[1];

            return parameters;
        }
    }

    public static void Initialize()
    {
        string path = System.AppDomain.CurrentDomain.BaseDirectory + "/scr.cfg";

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < EncryptionKey.Length; i++)
            {
                EncryptionKey[i] = lines[i];
            }

            return;
        }

        NETCore.Encrypt.Internal.AESKey aes = EncryptProvider.CreateAesKey();
        EncryptionKey[0] = aes.Key;
        EncryptionKey[1] = aes.IV;

        string content = aes.Key + "\n" + aes.IV;
        File.WriteAllText(path, content);
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
