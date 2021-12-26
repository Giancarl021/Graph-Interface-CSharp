using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace GraphInterface.Services.Converters
{
    internal class HttpMethodJsonConverter : JsonConverter<HttpMethod>
    {
        public override HttpMethod ReadJson(JsonReader reader, Type objectType, HttpMethod existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!hasExistingValue)
            {
                return null;
            }

            return new HttpMethod(reader.Value.ToString());
        }
        public override void WriteJson(JsonWriter writer, HttpMethod value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}