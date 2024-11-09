using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using NETCore.Encrypt;
using System.Text;
using NETCore.Encrypt.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public static class Security
{
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
        return "";
        //return EncryptProvider.AESEncrypt(text, PhoneNumberEncryptionKey.Key, PhoneNumberEncryptionKey.IV);
    }

    public static string DecryptText(string text)
    {
        return "";
        //return EncryptProvider.AESDecrypt(text, PhoneNumberEncryptionKey.Key, PhoneNumberEncryptionKey.IV);
    }
}
