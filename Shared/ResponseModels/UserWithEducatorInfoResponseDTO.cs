using Shared.Models;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class UserWithEducatorInfoResponseDTO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Address { get; set; }
        public long? ProvinceId { get; set; }
        public long? ProvinceKpsCode { get; set; }
        public string ProvinceName { get; set; }
        public string Nationality { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
        public virtual InstitutionResponseDTO Institution { get; set; }
        public long? InstitutionId { get; set; }
        public virtual List<EducatorResponseDTO> Educators { get; set; }
        public KPSResultResponseDTO KPSResult { get; set; }
        public CKYSDoctor CKYSResult { get; set; }
        public AcademicAdminStaffResponseDTO YOKResult { get; set; }
        public List<GraduationDetailResponseDTO> EgitimBilgisiResult { get; set; }
    }
}
