﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    internal class StringToIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int result;

            if (!int.TryParse(reader.GetString(), out result))
                return null;

            return result;
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
