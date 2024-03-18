using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Template4337
{
    public class Converters : JsonConverter<DateTime?>
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
    public class StringToIntConverter : JsonConverter<int?>
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
    public class StringToTimeSpanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            var time = value.Split(new char[] { ':' });

            if (time.Length != 2)
            {
                return null;
            }

            int hour, minute;

            if (!int.TryParse(time[0], out hour) || !int.TryParse(time[1], out minute))
            {
                return null;
            }

            var result = new System.TimeSpan(hour, minute, 0);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class IntToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
