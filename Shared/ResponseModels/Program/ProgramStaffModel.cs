namespace Shared.ResponseModels.Program;

public class ProgramStaffModel
{
    public long Id { get; set; }

    public string ProvinceName { get; set; }
    public long? ProvinceId { get; set; }
    public string ParentInstitutionName { get; set; }
    public long? ParentInstitutionId { get; set; }
    public string ProfessionName { get; set; }
    public long? ProfessionId { get; set; }
    public string UniversityName { get; set; }
    public long? UniversityId { get; set; }
    public string FacultyName { get; set; }
    public long? FacultyId { get; set; }
    public string HospitalName { get; set; }
    public long? HospitalId { get; set; }
    public string ExpertiseBranchName { get; set; }
    public long? ExpertiseBranchId { get; set; }

    public int EmployeeCount { get; set; }
    public int EducatorCount { get; set; }
    public int StudentCount { get; set; }
}