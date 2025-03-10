using System;
using System.Collections.Generic;

namespace Shared.BaseModels;

public class AuthorizationDetailBase
{
    public long? Id { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public DateTime? AuthorizationEndDate { get; set; }
    public DateTime? VisitDate { get; set; }
    public string AuthorizationDecisionNo { get; set; }
    public List<string> Descriptions { get; set; }
    public string Order { get; set; }

    public long? ProgramId { get; set; }
    public long? AuthorizationCategoryId { get; set; }
}