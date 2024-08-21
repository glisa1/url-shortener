using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Utils;

public static class HashComputer
{
    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new();
        foreach (byte b in ComputeHashForUrl(inputString))
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }

    private static byte[] ComputeHashForUrl(string url)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(url));
    }
}
