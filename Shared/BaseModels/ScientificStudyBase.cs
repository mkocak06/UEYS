using System;

namespace Shared.BaseModels;

public class ScientificStudyBase
{
    public DateTime? ProcessDate { get; set; }
    public string Description { get; set; }

    public long? StudentId { get; set; }
    public long? StudyId { get; set; }
}