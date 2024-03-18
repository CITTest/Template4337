using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    internal class ConvertToInt : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = reader.GetInt64();

            return Convert.ToString(result);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
