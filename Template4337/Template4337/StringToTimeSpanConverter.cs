using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    public class StringToTimeSpanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value =  reader.GetString();
            
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
}