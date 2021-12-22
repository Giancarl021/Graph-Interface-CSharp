using System.Security.Cryptography;
using System.Text;
using GraphInterface.Models.Abstract;

namespace GraphInterface.Services
{
    internal class GraphInterfaceRequestHasher
    {
        public static string Hash(string uri, GraphInterfaceRequestOptions options)
        {
            var properties = options.GetType().GetProperties();
            string data = uri + "@";

            foreach (var property in properties)
            {
                data += $"{property.Name}::{property.GetValue(options)?.ToString()}|";
            }

            data = data.Remove(data.Length - 1);

            return HashWithSHA1(data);
        }

        private static string HashWithSHA1(string input)
        {
            using (HashAlgorithm hash = SHA1.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

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