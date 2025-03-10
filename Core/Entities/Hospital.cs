using Core.Entities.Koru;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Hospital : ExtendedBaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string WebAddress { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int Code { get; protected set; }
        public string CKYSCode { get; set; }

        public long? ProvinceId { get; set; }
        public virtual Province Province { get; set; }
        public long? InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
        public long? FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }

        public virtual ICollection<Program> Programs { get; set; }
        public virtual ICollection<Affiliation> Affiliations { get; set; }
        public virtual ICollection<ThesisDefence> ThesisDefences { get; set; }
        public virtual ICollection<ExitExam> ExitExams { get; set; }
        public virtual ICollection<EducatorStaffInstitution> EducatorStaffInstitutions { get; set; }
        public virtual ICollection<UserRoleHospital> UserRoleHospitals { get; set; }
        public virtual ICollection<QuotaRequest> QuotaRequests { get; set; }
    }
}
