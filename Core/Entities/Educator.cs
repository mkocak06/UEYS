using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Educator : ExtendedBaseEntity
    {
        public long? StaffTitleId { get; set; }
        public virtual Title StaffTitle { get; set; }
        public long? AcademicTitleId { get; set; }
        public virtual Title AcademicTitle { get; set; }
        public long? UserId { get; set; }
        public virtual User User { get; set; }

        public bool? IsConditionalEducator { get; set; }
        public bool? IsForensicMedicineInstitutionEducator { get; set; }
        public bool? IsChairman { get; set; }
        public ForensicMedicineBoardType? ForensicMedicineBoardType { get; set; }
        public DateTime? MembershipStartDate { get; set; }
        public DateTime? MembershipEndDate { get; set; }
        public EducatorType EducatorType { get; set; } = EducatorType.Instructor;
        public EducatorDeleteReasonType? DeleteReason { get; set; }
        public string DeleteReasonExplanation { get; set; }
        public DateTime? TitleDate { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<EducatorProgram> EducatorPrograms { get; set; }
        public virtual ICollection<EducatorExpertiseBranch> EducatorExpertiseBranches { get; set; }
        public virtual ICollection<EducatorDependentProgram> EducatorDependentPrograms { get; set; }
        public virtual ICollection<Thesis> Theses { get; set; }
        public virtual ICollection<PerformanceRating> PerformanceRatings { get; set; }
        public virtual ICollection<Jury> Juries { get; set; }
        public virtual ICollection<AdvisorThesis> AdvisorTheses { get; set; }
        public virtual ICollection<EducationTracking> EducationTrackings { get; set; }
        public virtual ICollection<OpinionForm> EducatorOpinionForms { get; set; }
        public virtual ICollection<OpinionForm> ProgramManagerOpinionForms { get; set; }
        public virtual ICollection<StudentRotation> StudentRotations { get; set; }
        public virtual ICollection<StudentPerfection> StudentPerfections { get; set; }
        public virtual ICollection<StudentRotationPerfection> StudentRotationPerfections { get; set; }
        public virtual ICollection<ProgressReport> ProgressReports { get; set; }
        public virtual ICollection<EducatorAdministrativeTitle> EducatorAdministrativeTitles { get; set; }
        public virtual ICollection<GraduationDetail> GraduationDetails { get; set; }
        public virtual ICollection<EducationOfficer> EducationOfficers { get; set; }
        public virtual ICollection<EducatorStaffInstitution> StaffInstitutions { get; set; }
        public virtual ICollection<EducatorStaffParentInstitution> StaffParentInstitutions { get; set; }
    }
}
