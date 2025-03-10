using System;
using Shared.Types;

namespace Shared.BaseModels;

public class StudentBase
{
    public string DeleteReason { get; set; }
	//public StudentDeleteReasonType? DeleteReasonType{ get; set; }
	public ReasonType? DeleteReasonType{ get; set; }
    public StudentStatus? Status { get; set; }
    public string GraduatedSchool { get; set; }
    public string GraduatedDate { get; set; }
    public string MedicineRegistrationDate { get; set; }
    public string MedicineRegistrationNo { get; set; }
    public PlacementExamType? BeginningExam { get; set; }
    public int? BeginningYear { get; set; }
    public PeriodType? BeginningPeriod { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsHardDeleted { get; set; }
    public DateTime? DeleteDate { get; set; }
    public double? PlacementScore { get; set; }
    public QuotaType? QuotaType { get; set; }
    public QuotaType_1? QuotaType_1 { get; set; }
    public QuotaType_2? QuotaType_2 { get; set; }
    public bool? TransferredDueToOpinion { get; set; }
    public string OrcidNumber { get; set; }
    public string RegistrationStatements { get; set; }

    public long? UserId { get; set; }
    public long? ProgramId { get; set; }
    public long? CurriculumId { get; set; }
    public long? OriginalProgramId { get; set; }
    public long? ProtocolProgramId { get; set; }
    public ProgramType? ProtocolOrComplement{ get; set; }

    public long? AdvisorId { get; set; }

    //Add Student
    public bool? IsTransferred { get; set; }
    //public bool? StartedWithAmnestyLaw { get; set; }
    public ReasonType? StartReason { get; set; }
    public DateTime? FirstInsStartDate { get; set; }
    public DateTime? FirstInsLeavingDate { get; set; }
    public long? TransferProgramId { get; set; }
    public ReasonType? TransferReasonType { get; set; }
    public ExcusedType? ExcusedType { get; set; }

}