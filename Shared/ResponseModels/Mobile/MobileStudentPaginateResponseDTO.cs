using Shared.Types;
using System;

namespace Shared.ResponseModels.Mobile
{
    public class MobileStudentPaginateResponseDTO
    {
        public long? Id { get; set; }
        public string IdentityNo { get; set; }
        public string Name { get; set; }
        public GenderType? Gender { get; set; }
        public string ProfessionName { get; set; }
        public string HospitalName { get; set; }
        public string ExpertiseBranchName { get; set; }
        public string CurriculumVersion { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? UserIsDeleted { get; set; }
        public string DeleteReason { get; set; }
        public long? AdvisorId { get; set; }
        public long? CurriculumId { get; set; }
        public long? ProgramId { get; set; }
        public long? HospitalId { get; set; }
        public long? UniversityId { get; set; }
        public long? UserId { get; set; }
        public StudentStatus? Status { get; set; }
        public long? OriginalProgramId { get; set; }
        public long? OriginalHospitalId { get; set; }
        public long? OriginalUniversityId { get; set; }
        public string OriginalHospitalName { get; set; }
        public string OriginalExpertiseBranchName { get; set; }
        public long? FormerProgramId { get; set; }
        public long? FormerHospitalId { get; set; }
        public long? FormerUniversityId { get; set; }
        public string FormerHospitalName { get; set; }
        public string FormerExpertiseBranchName { get; set; }
        public long? ProtocolProgramId { get; set; }
        public string ProtocolExpertiseBranchName { get; set; }
        public string ProtocolHospitalName { get; set; }
        public ProgramType? ProtocolType { get; set; }
        public int? RemainingDays { get; set; }
        public DateTime? BeginningDate { get; set; }


        public DateTime? EstimatedFinish { get; set; }

        public bool? IsThesis { get; set; }

        public string ThesisResult
        {
            get
            {
                return Thesis.Status == null ? "-" : (Thesis.Status == ThesisStatusType.Continues ? "Devam Ediyor" : (Thesis.Status == ThesisStatusType.Successful ? "Başarılı" : "Başarısız"));
            }
        }
        public ThesisResponseDTO? Thesis { get; set; }
        public string ExitExamResult
        {
            get
            {
                return ExitExam.ExamStatus == null ? "-" : (ExitExam.ExamStatus == ExitExamResultType.NotConcluded ? "Sonuçlanmadı" : (ExitExam.AbilityExamNote >= 60 && ExitExam.PracticeExamNote >= 60 ? "Başarılı" : "Başarısız"));
            }
        }
        public ExitExamResponseDTO? ExitExam { get; set; }
        public QuotaType? QuatoType { get; set; }
        public QuotaType_1? QuatoType1 { get; set; }
        public QuotaType_2? QuatoType2 { get; set; }
    }
}
