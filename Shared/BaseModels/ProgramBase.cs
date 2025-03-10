using System;

namespace Shared.BaseModels;

public class ProgramBase
{
    public long Id { get; set; }

    public long? FacultyId { get; set; }
    public long? HospitalId { get; set; }
    public long? ExpertiseBranchId { get; set; }
    public long? AffiliationId { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeleteDate { get; set; }
    public long? ProtocolProgramId { get; set; }
    public long? ParentProgramId { get; set; }
    public string Code { get; set; }

}