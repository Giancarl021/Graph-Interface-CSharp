using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphInterface.Services.Converters
{
    internal class HttpMethodJsonConverter : JsonConverter<HttpMethod>
    {
        public override HttpMethod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value is null) return null;
            return new HttpMethod(value.ToUpperInvariant());
        }
        public override void Write(Utf8JsonWriter writer, HttpMethod value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}