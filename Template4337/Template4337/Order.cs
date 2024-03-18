using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Template4337
{
    public partial class Order
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        [JsonPropertyName("CodeOrder")]
        public string OrderCode { get; set; }
        
        [JsonPropertyName("CreateDate")]
        [JsonConverter(typeof(Converters))]
        public DateTime? DateCreate { get; set; }
        
        [JsonPropertyName("CreateTime")]
        [JsonConverter(typeof(StringToTimeSpanConverter))]
        public TimeSpan? TimeCreate { get; set; }
        
        [JsonPropertyName("CodeClient")]
        [JsonConverter(typeof(StringToIntConverter))]
        public int? ClentCode { get; set; }
        
        [JsonPropertyName("Services")]
        public string Uslugi { get; set; }
        
        [JsonPropertyName("Status")]
        public string Status { get; set; }
        
        [JsonPropertyName("ClosedDate")]
        [JsonConverter(typeof(Converters))]
        public DateTime? DateOfEnd { get; set; }
        
        [JsonPropertyName("ProkatTime")]
        [JsonConverter(typeof(IntToStringConverter))]
        public string TimeOfProcat { get; set; }
    }
}
