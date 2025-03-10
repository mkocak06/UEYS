using System;

namespace Core.Models
{
    public class KPSResult
    {
        public long TCKN { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDay { get; set; }
        public string BirthPlace { get; set; }
        public string Nationality { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public AddressInfo AddressInfo { get; set; }
    }
    public class AddressInfo
    {
        public string Address { get; set; }
        public int? ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}
