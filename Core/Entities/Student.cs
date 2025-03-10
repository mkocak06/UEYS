using Core.Entities.Koru;
using Shared.Types;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Student : ExtendedBaseEntity
    {
        public string DeleteReason { get; set; }
		//public StudentDeleteReasonType? DeleteReasonType { get; set; }
		public ReasonType? DeleteReasonType { get; set; }
        public StudentStatus? Status { get; set; }
        public string GraduatedSchool { get; set; }
        public string GraduatedDate { get; set; }
        public string MedicineRegistrationNo { get; set; }
        public string MedicineRegistrationDate { get; set; }
        public PlacementExamType? BeginningExam { get; set; }
        public int? BeginningYear { get; set; }
        public PeriodType? BeginningPeriod { get; set; }
        public double? PlacementScore { get; set; }
        public QuotaType? QuotaType { get; set; }
        public QuotaType_1? QuotaType_1 { get; set; }
        public QuotaType_2? QuotaType_2 { get; set; }
        public bool? TransferredDueToOpinion { get; set; }
        public string OrcidNumber { get; set; }
        public string RegistrationStatements { get; set; }
        public bool? ConditionallyGraduated{ get; set; }

        public long? UserId { get; set; }
        public virtual User User { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public long? OriginalProgramId { get; set; }
        public virtual Program OriginalProgram { get; set; }
        public long? ProtocolProgramId { get; set; }
        public virtual Program ProtocolProgram { get; set; }
        public long? CurriculumId { get; set; }
        public virtual Curriculum Curriculum { get; set; }
        public bool IsHardDeleted { get; set; } = false;
        public long? AdvisorId { get; set; }
        public virtual Educator Advisor { get; set; }

        public virtual ICollection<StudentRotation> StudentRotations { get; set; }
        public virtual ICollection<StudentPerfection> StudentPerfections { get; set; }
        public virtual ICollection<Thesis> Theses { get; set; }
        public virtual ICollection<PerformanceRating> PerformanceRatings { get; set; }
        public virtual ICollection<OpinionForm> OpinionForms { get; set; }
        public virtual ICollection<EducationTracking> EducationTrackings { get; set; }
        public virtual ICollection<StudentExpertiseBranch> StudentExpertiseBranches { get; set; }
        public virtual ICollection<ScientificStudy> ScientificStudies { get; set; }
        public virtual ICollection<ExitExam> ExitExams { get; set; }
        public virtual ICollection<UserRoleStudent> UserRoleStudents { get; set; }
        public virtual ICollection<StudentDependentProgram> StudentDependentPrograms{ get; set; }

        public virtual ICollection<StudentsSpecificEducation> StudentsSpecificEducations { get; set; }
    }
}
