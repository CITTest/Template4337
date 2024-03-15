using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    public class StringToDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            
            if (string.IsNullOrEmpty(value))
                return null;
            
            var date = value.Split(new char[] { '.' });
            
            if (date.Length != 3)
                return null;
            
            int day, month, year;
            
            if (!int.TryParse(date[0], out day) || !int.TryParse(date[1], out month) ||
                !int.TryParse(date[2], out year))
                return null;
            
            var result = new DateTime(year, month, day);
            
            return result;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}