using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    internal class Conveter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            DateTime result;

            if (!DateTime.TryParse(reader.GetString(), out result))
                result = DateTime.UtcNow;

            return result;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
