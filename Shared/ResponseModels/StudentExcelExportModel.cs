using System;
using Shared.Types;

namespace Shared.ResponseModels;

public class StudentExcelExportModel
{
    public bool? IsDeleted { get; set; }
    public bool? UserIsDeleted { get; set; }

    public string IdentityNo { get; set; }
    public string Name { get; set; }
    public string Nationality { get; set; }
    public GenderType? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string ProfessionName { get; set; }
    public string OriginalHospitalName { get; set; }
    public string OriginalUniversityName { get; set; }
    public bool? OriginalUniversityIsPrivate { get; set; }
    public string OriginalInstitutionName { get; set; }
    public long? OriginalInstitutionId { get; set; }
    public string OriginalExpertiseBranchName { get; set; }
    public bool? ExpertiseBranchIsPrincipal { get; set; }
    public string ProgramCategory { get; set; }
    public QuotaType? QuatoType { get; set; }
    public QuotaType_1? QuatoType1 { get; set; }
    public QuotaType_2? QuatoType2 { get; set; }
    public PeriodType? BeginningPeriod { get; set; }
    public PlacementExamType? BeginningExam { get; set; }
    public int? BeginningYear { get; set; }
    public DateTime? BeginningDate { get; set; }
    public DateTime? EstimatedFinish { get; set; }
    public int? RemainingDays { get; set; }
    public ThesisStatusType? ThesisStatusType { get; set; }
    public string ThesisResult {
        get
        {
            return ThesisStatusType == null ? "-" : (ThesisStatusType == Types.ThesisStatusType.Continues ? "Devam Ediyor" : (ThesisStatusType == Types.ThesisStatusType.Successful ? "Başarılı" : "Başarısız"));  
        }
    }
    public ExitExamResultType? ExamStatus { get; set; }
    public double? AbilityExamNote { get; set; }
    public double? PracticeExamNote { get; set; }
    public string ExitExamResult
    {
        get
        {
            return ExamStatus == null ? "-" : (ExamStatus == ExitExamResultType.NotConcluded ? "Sonuçlanmadı" : (AbilityExamNote >= 60 && PracticeExamNote >= 60 ? "Başarılı" : "Başarısız"));
        }
    }
    public string CurriculumVersion { get; set; }
    public StudentStatus? Status { get; set; }
    public string ProvinceName { get; set; }
    public bool? IsThesisSubjectDetermined { get; set; }
    public DateTime? ThesisAdvisorAssingnDate { get; set; }
    public DateTime? ThesisSubjectDetermineDate { get; set; }

    // E-nabiz
    public int? MuayeneSayisi { get; set; }
    public int? ReceteSayisi { get; set; }
    public int? IslemSayisi { get; set; }
    public int? LaboratuvarTetkikİstemiSayisi { get; set; }

    public int? ATypeSurgeryCount { get; set; }
    public int? BTypeSurgeryCount { get; set; }
    public int? CTypeSurgeryCount { get; set; }
    public int? DTypeSurgeryCount { get; set; }
    public int? ETypeSurgeryCount { get; set; }
}