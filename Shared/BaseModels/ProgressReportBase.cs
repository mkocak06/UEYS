using System;

namespace Shared.BaseModels;

public class ProgressReportBase
{
    public long Id { get; set; }
    public string Description { get; set; }
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }

    public long? ThesisId { get; set; }
    public long? EducatorId { get; set; }
}