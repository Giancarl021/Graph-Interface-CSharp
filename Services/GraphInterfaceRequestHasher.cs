using System.Security.Cryptography;
using System.Text;
using GraphInterface.Models.Abstract;
using Newtonsoft.Json;

namespace GraphInterface.Services
{
    internal class GraphInterfaceRequestHasher
    {
        public static string Hash(string uri, GraphInterfaceRequestOptions options)
        {
            string data = uri + "::" + JsonConvert.SerializeObject(options);

            using (HashAlgorithm hash = SHA1.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}