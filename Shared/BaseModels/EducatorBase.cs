using System;
using System.Collections.Generic;
using Shared.RequestModels;
using Shared.Types;

namespace Shared.BaseModels;

public class EducatorBase
{
    public long? Id { get; set; }
    public string DeleteReasonExplanation { get; set; }
    public EducatorDeleteReasonType DeleteReason { get; set; }
    public EducatorType EducatorType { get; set; }
    public NonInstructorType? NonInstructorType { get; set; }
    public bool? IsDeleted { get; set; }
    public long? StaffTitleId { get; set; }
    public string StaffTitleName { get; set; }
    public long? AcademicTitleId { get; set; }
    public string AcademicTitleName { get; set; }
    public long? UserId { get; set; }
    public DateTime? TitleDate { get; set; }
    public bool? IsConditionalEducator { get; set; }
    public bool? IsForensicMedicineInstitutionEducator { get; set; }
    public bool? IsChairman { get; set; }
    public ForensicMedicineBoardType? ForensicMedicineBoardType { get; set; }
    public string AdministrativeTitlesForEducatorTag { get; set; }
    public DateTime? MembershipStartDate { get; set; }
    public DateTime? MembershipEndDate { get; set; }

}