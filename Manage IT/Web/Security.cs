using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

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
}
