using Shared.Models;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class UserWithStudentInfoResponseDTO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ProfilePhoto { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Address { get; set; }
        public long? ProvinceId { get; set; }
        public long? ProvinceKpsCode { get; set; }
        public string ProvinceName { get; set; }
        public string Nationality { get; set; }
        public ForeignType? ForeignType { get; set; }
        public long? CountryId { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
        public List<StudentResponseDTO> Students { get; set; }
        public KPSResultResponseDTO KPSResult { get; set; }
        public CKYSStudent CKYSResult { get; set; }
    }
}
