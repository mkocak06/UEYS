using Core.Entities.Koru;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Program : ExtendedBaseEntity
    {
        public long? ManagerId { get; set; }
        public virtual User Manager { get; set; }
        public long? FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }
        public long? HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public string Code { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }
        public virtual ICollection<ProtocolProgram> ParentPrograms { get; set; }

        public virtual ICollection<AuthorizationDetail> AuthorizationDetails { get; set; }
        public virtual ICollection<EducatorProgram> EducatorPrograms { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Student> OriginalStudents { get; set; }
        public virtual ICollection<Student> ProtocolStudents { get; set; }
        public virtual ICollection<DependentProgram> DependentPrograms { get; set; }
        public virtual ICollection<EducationTracking> EducationTrackingsFormer { get; set; }
        public virtual ICollection<EducationTracking> EducationTrackings { get; set; }
        public virtual ICollection<StudentRotation> StudentRotations { get; set; }
        public virtual ICollection<StudentPerfection> StudentPerfections { get; set; }
        public virtual ICollection<UserRoleProgram> UserRolePrograms { get; set; }
        public virtual ICollection<OpinionForm> OpinionForms { get; set; }
        public virtual ICollection<EducationOfficer> EducationOfficers { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
        public virtual ICollection<SubQuotaRequest> SubQuotaRequests { get; set; }
    }
}
