using System;

namespace Shared.BaseModels;

public class AffiliationBase
{
    public string ProtocolNo { get; set; }
    public DateTime? ProtocolDate { get; set; }
    public DateTime? ProtocolEndDate { get; set; }

    public long? FacultyId { get; set; }
    public long? HospitalId { get; set; }
}