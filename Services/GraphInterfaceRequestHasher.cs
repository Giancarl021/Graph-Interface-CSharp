using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GraphInterface.Models.Abstract;

namespace GraphInterface.Services;
internal static class GraphInterfaceRequestHasher
{
    public static string Hash(string uri, GraphInterfaceRequestOptions options)
    {
        string data = uri + "::" + JsonSerializer.Serialize(options);
        byte[] bytes = SHA1.HashData(Encoding.UTF8.GetBytes(data));

        StringBuilder builder = new StringBuilder();

        foreach (byte b in bytes)
            builder.Append(b.ToString("x2"));

        return builder.ToString();
    }
}