namespace Shared.ResponseModels.StatisticModels;

public class ReportResponseDTO
{
    public string Key { get; set; }
    public string Value { get; set; }
    public bool? IsPrivate { get; set; }
    public long ParentInstitutionId { get; set; }
}