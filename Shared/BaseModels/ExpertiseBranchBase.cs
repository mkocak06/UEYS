using Shared.RequestModels;
using System.Collections.Generic;

namespace Shared.BaseModels;

public class ExpertiseBranchBase
{
    public long? Id { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public bool? IsPrincipal { get; set; }
    public int? ProtocolProgramCount { get; set; }
    public bool? IsIntensiveCare { get; set; }
    public int Code { get; set; }
    public int? PortfolioIndexRateToCapacityIndex { get; set; }
    public int? EducatorIndexRateToCapacityIndex { get; set; }

    public long? ProfessionId { get; set; }
    public string CKYSCode { get; set; }
    public string SKRSCode { get; set; }
}