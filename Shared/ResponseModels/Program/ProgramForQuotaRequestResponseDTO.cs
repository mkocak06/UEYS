using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels.Program
{
    public class ProgramForQuotaRequestResponseDTO
    {
        public long Id { get; set; }

        public string HospitalName { get; set; }
        public long? HospitalId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsQuotaRequestable { get; set; }
        public List<string> ExpertiseBranchPortfolioNames { get; set; }


        public ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
        public List<SubQuotaRequestResponseDTO> SubQuotaRequests { get; set; }
    }
}
