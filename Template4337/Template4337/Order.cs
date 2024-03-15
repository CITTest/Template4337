using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Template4337
{
    public partial class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public DateTime? DateCreate { get; set; }
        public TimeSpan? TimeCreate { get; set; }
        public int? ClentCode { get; set; }
        public string Uslugi { get; set; }
        public string Status { get; set; }
        public DateTime? DateOfEnd { get; set; }
        public string TimeOfProcat { get; set; }
    }
}
