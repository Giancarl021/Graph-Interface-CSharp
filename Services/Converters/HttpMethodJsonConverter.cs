using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace GraphInterface.Services.Converters
{
    internal class HttpMethodJsonConverter : JsonConverter<HttpMethod>
    {
        public override HttpMethod? ReadJson(JsonReader reader, Type objectType, HttpMethod? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!hasExistingValue)
                return null;

            return reader.Value != null ? new HttpMethod(reader.Value!.ToString()!) : null;
        }
        public override void WriteJson(JsonWriter writer, HttpMethod? value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value.ToString());
        }
    }
}