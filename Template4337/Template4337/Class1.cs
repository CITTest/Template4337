using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    public class class1
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("NameServices")]
        public string Name { get; set; }
        [JsonPropertyName("TypeOfService")]
        public string View { get; set; }
        [JsonPropertyName("CodeService")]
        public string Code { get; set; }

        [JsonPropertyName("Cost")]
        public int? Price { get; set; }
        public int Group { get; set; }

        public class1()
        {

        }
        public class1(string  name, string view, string code, int price)
        {
            Name = name;
            View = view;
            Code = code;
            Price = price;

            if (Price < 351) Group = 0;
            if (Price > 350 && Price < 800) Group = 1;
            if (Price > 800) Group = 2;
        }
        public void checkGroup()
        {
            if (Price < 351) Group = 0;
            if (Price > 350 && Price < 800) Group = 1;
            if (Price > 800) Group = 2;
        }
    }
    public partial class Context : DbContext
    {
        public virtual DbSet<class1> Class1s { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=isrpo3;Trusted_Connection=True;");
            }
        }
        public Context() => Database.EnsureCreated();
    }

    // JSON

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
