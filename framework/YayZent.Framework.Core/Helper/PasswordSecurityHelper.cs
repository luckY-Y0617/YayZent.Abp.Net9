using System.Security.Cryptography;

namespace YayZent.Framework.Core.Helper;

public class PasswordSecurityHelper
{
    private const int SaltSize = 16; // 128 位
    private const int HashSize = 32; // 256 位
    private const int Iterations = 100_000;

    // 生成盐值
    public static byte[] GenerateSalt()
    {
        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    // 将 byte[] 转为 hex string（与 HashHelper 中方法相似）
    private static string BytesToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    // 将 hex string 转为 byte[]
    private static byte[] HexToBytes(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }

    // 创建密码哈希 + 盐（建议保存 salt 和 hash）
    public static (string HashHex, string SaltHex) HashPassword(string password)
    {
        byte[] salt = GenerateSalt();
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        return (BytesToHex(hash), BytesToHex(salt));
    }

    // 验证密码是否匹配
    public static bool VerifyPassword(string password, string hashHex, string saltHex)
    {
        byte[] salt = HexToBytes(saltHex);
        byte[] expectedHash = HexToBytes(hashHex);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] actualHash = pbkdf2.GetBytes(HashSize);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}