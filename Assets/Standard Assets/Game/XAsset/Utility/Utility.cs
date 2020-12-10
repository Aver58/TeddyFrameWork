using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public static class Utility
{
    private static readonly MD5 md5 = MD5.Create();
    private static readonly CRC32 crc32 = new CRC32();

    public static string GetMD5Hash(string input)
    {
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return ToHash(data);
    }

    public static string GetMD5Hash(Stream input)
    {
        var data = md5.ComputeHash(input);
        return ToHash(data);
    }

    public static bool VerifyMd5Hash(string input, string hash)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        return 0 == comparer.Compare(input, hash);
    }

    public static string GetCRC32Hash(Stream input)
    {
        var data = crc32.ComputeHash(input);
        return ToHash(data);
    }

    public static uint GetCrc(byte[] bytes)
    {
        return CRC32.Compute(bytes);
    }

    public static string GetCRC32Hash(byte[] bytes)
    {
        var data = crc32.ComputeHash(bytes);
        return ToHash(data);
    }

    static string ToHash(byte[] data)
    {
        var sb = new StringBuilder();
        foreach(var t in data)
            sb.Append(t.ToString("x2"));
        return sb.ToString();
    }

    public static string GetCRC32Hash(string input)
    {
        var data = crc32.ComputeHash(Encoding.UTF8.GetBytes(input));
        return ToHash(data);
    }

    public static bool VerifyCrc32Hash(string input, string hash)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        return 0 == comparer.Compare(input, hash);
    }
}