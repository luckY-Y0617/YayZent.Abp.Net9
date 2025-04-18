using System.Security.Cryptography;
using System.Text;

namespace YayZent.Framework.Core.Helper;

public static class HashHelper
{
    private static string ToString(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    public static string ComputeSha256Hash(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        return ToString(bytes);
    }

    public static string ComputeSha256Hash(Stream input)
    {
        using SHA256 sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(input);
        return ToString(bytes);
    }
}