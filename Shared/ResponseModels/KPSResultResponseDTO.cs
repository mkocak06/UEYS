using Shared.Types;
using System;

namespace Shared.ResponseModels
{
    public class KPSResultResponseDTO
    {
        public long TCKN { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDay { get; set; }
        public string BirthPlace { get; set; }
        public string Nationality { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public AddressInfoResponseDTO AddressInfo { get; set; }
    }
}
