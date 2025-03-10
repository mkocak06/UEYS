using Core.Entities.Koru;
using System.Collections.Generic;

namespace Core.Entities
{
    public class University : ExtendedBaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public string Email { get; set; }
        public bool? IsPrivate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string CKYSCode { get; set; }

        public long? ManagerId { get; set; }
        public virtual User Manager { get; set; }

        public long? ProvinceId { get; set; }
        public virtual Province Province { get; set; }
        public long? InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }

        public virtual ICollection<Affiliation> Affiliations { get; set; }
        public virtual ICollection<Faculty> Faculties { get; set; }
        public virtual ICollection<UserRoleUniversity> UserRoleUniversities { get; set; }
        public virtual ICollection<EducatorStaffParentInstitution> StaffEducators { get; set; }
    }
}
