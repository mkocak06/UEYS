namespace Core.Models.DetailedReportModels;

public class CountByExpertiseBranch
{
    public string ExpertiseBranchName { get; set; }
    public string ProfessionName { get; set; }
    public bool IsPrincipal { get; set; }
    public int ProgramCount { get; set; }
    public int EducatorCount { get; set; }
    public int StudentCount { get; set; }
}