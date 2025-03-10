using System;

namespace Shared.BaseModels;

public class StudentExpertiseBranchBase
{
    public long? Id { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string RegistrationNo { get; set; }
    public string ExpertiseBranchName { get; set; }
    public bool? IsPrincipal { get; set; }

    public long? ExpertiseBranchId { get; set; }
    public long? StudentId { get; set; }
}