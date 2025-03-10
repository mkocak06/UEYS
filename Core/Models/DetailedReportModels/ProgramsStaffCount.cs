using System;
using System.Collections.Generic;

namespace Core.Models.DetailedReportModels;

public class ProgramsStaffCount
{
    public long Id { get; set; }
    public string ProfessionName { get; set; }
    public string ProvinceName { get; set; }
    public string ParentInstitutionName { get; set; }
    public string UniversityName { get; set; }
    public string FacultyName { get; set; }
    public string HospitalName { get; set; }
    public bool? IsPrivate { get; set; }
    public string ExpertiseBranchName { get; set; }
    public string AffiliatedUniversityName { get; set; }
    public string AffiliatedHospitalName { get; set; }
    public string AuthorizationCategory { get; set; }
    public string AuthorizationCategoryColorCode { get; set; }
    public IEnumerable<string> EducatorList { get; set; }
    public IEnumerable<DateTime?> StudentList { get; set; }
}