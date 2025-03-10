using System;

namespace Shared.ResponseModels.ENabiz;

public class StudentResponseDTO
{
    public string Name { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime? CreateDate { get; set; }
    public long? ExpertiseBranchCode { get; set;}
    public string ExpertiseBranchName { get; set; }
    public DateTime? GraduationDate { get; set; }
}