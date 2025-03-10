using System;
using System.Collections.Generic;

namespace Shared.BaseModels;

public class EducatorExpertiseBranchBase
{
    public long? Id { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string RegistrationNo { get; set; }
    public List<long> SubBranchIds { get; set; }
    public string RegistrationBranchName { get; set; }
    public string RegistrationGraduationSchool { get; set; }
    public string ExpertiseBranchName { get; set; }
    //public bool? IsPrincipal { get; set; }

    public long? ExpertiseBranchId { get; set; }
    public long? EducatorId { get; set; }
}