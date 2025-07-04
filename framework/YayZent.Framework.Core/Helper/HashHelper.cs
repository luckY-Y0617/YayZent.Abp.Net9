using System.Security.Cryptography;
using System.Text;

namespace YayZent.Framework.Core.Helper;

public static class HashHelper
{
    public enum HashEncoding
    {
        Hex,
        Base64
    }

    private static string ToHexString(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    private static string ToBase64String(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }

    public static string ComputeSha256Hash(string input, HashEncoding encoding = HashEncoding.Hex)
    {
        using SHA256 sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(inputBytes);

        return encoding == HashEncoding.Base64 ? ToBase64String(hashBytes) : ToHexString(hashBytes);
    }

    public static string ComputeSha256Hash(Stream input, HashEncoding encoding = HashEncoding.Hex)
    {
        using SHA256 sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(input);
        return encoding == HashEncoding.Base64 ? ToBase64String(hashBytes) : ToHexString(hashBytes);
    }
}