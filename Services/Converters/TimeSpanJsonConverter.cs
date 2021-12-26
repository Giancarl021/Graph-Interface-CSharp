using System;
using Newtonsoft.Json;

namespace GraphInterface.Services.Converters
{
    internal class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!hasExistingValue)
            {
                return TimeSpan.Zero;
            }

            return TimeSpan.FromSeconds(int.Parse(reader.Value.ToString()));
        }
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue(value.TotalSeconds.ToString());
        }
    }
}